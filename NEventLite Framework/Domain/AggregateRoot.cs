/****************************** Class Header ******************************\
Module Name:    <AggregateRoot.cs>
Project:        <NEventLite> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

What is an "AggregateRoot" in DDD? http://martinfowler.com/bliki/DDD_Aggregate.html

This will be the base class from which all of our other Aggregates inherit from.
All state changes here are driven through "Applying" of events/snapshots.
This way we can contruct the state for the aggregate to any point in time by replaying events.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using NEventLite.Events;
using NEventLite.Exceptions;

namespace NEventLite.Domain
{
    /// <summary>
    /// Base AggregateRoot class to inherit from
    /// </summary>
    public abstract class AggregateRoot
    {
        public enum StreamState
        {
            NoStream = -1,
            HasStream = 1
        }

        private const string ApplyMethodNameInEventHandler = "Apply";
        private readonly List<Events.Event> _uncommittedChanges;

        public Guid Id { get; protected set; } //The AggregateID must be unique
        public int CurrentVersion { get; protected set; } //This will store the current version of the aggregate
        public int LastCommittedVersion { get; protected set; } //We use this for implement optimistic concurrency

        public StreamState GetStreamState()
        {
            if (CurrentVersion == -1)
            {
                return StreamState.NoStream;
            }
            else
            {
                return StreamState.HasStream;
            }
        }

        protected AggregateRoot()
        {
            CurrentVersion = (int)StreamState.NoStream;
            LastCommittedVersion = (int)StreamState.NoStream;
            _uncommittedChanges = new List<Events.Event>();
        }

        /// <summary>
        /// Get the events that have been applied but not commited to storage
        /// </summary>
        /// <returns>The uncommited events</returns>
        public IEnumerable<Events.Event> GetUncommittedChanges()
        {
            return _uncommittedChanges.ToList();
        }

        /// <summary>
        /// This will mark all the new events as comitted to storage
        /// </summary>
        public void MarkChangesAsCommitted()
        {
            _uncommittedChanges.Clear();
            LastCommittedVersion = CurrentVersion;
        }

        /// <summary>
        /// This will reapply all the events
        /// </summary>
        /// <param name="history">The events to replay</param>
        public void LoadsFromHistory(IEnumerable<Events.Event> history)
        {
            foreach (var e in history)
            {
                //We call HandleEvent with isNew parameter set to false as we are replaying a historical event
                HandleEvent(e, false);
            }
            LastCommittedVersion = CurrentVersion;
        }

        /// <summary>
        /// This is used to handle new events
        /// </summary>
        /// <param name="event"></param>
        protected void HandleEvent(Events.Event @event)
        {
            HandleEvent(@event, true);
        }

        /// <summary>
        /// Finds the correct Apply() method in the AggregateRoot and call it to apply changes to state
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="isNew">Is this a new Event</param>
        private void HandleEvent(Events.Event @event, bool isNew)
        {
            //All state changes to AggregateRoot must happen via the Apply method
            //Make sure the right Apply method is called with the right type.
            //We can you use dynamic objects or reflection for this.

            try
            {
                var method = ((object)this).GetType().GetMethod(ApplyMethodNameInEventHandler, new Type[] { @event.GetType() }); //Find the right method
                method.Invoke(this, new object[] { @event }); //invoke with the event as argument

                // or we can use dynamics
                //dynamic d = this;
                //dynamic e = @event;
                //d.Apply(e);

                if (isNew)
                {
                    _uncommittedChanges.Add(@event); //only add to the events collection if it's a new event
                }
            }
            catch (Exception ex)
            {
                throw new EventHandlerApplyMethodMissingException(ex.Message);
            }

        }

        /// <summary>
        /// Determine if the current event can be applied
        /// </summary>
        /// <param name="event">Event to apply</param>
        /// <param name="isCreationEvent">Is this the first event for the aggregate</param>
        /// <returns></returns>
        private bool CanApply(Event @event, bool isCreationEvent)
        {

            //Check to see if event is applying against the right Aggregate and matches the target version
            if (((isCreationEvent) || (this.Id == @event.AggregateId)) && (this.CurrentVersion == @event.TargetVersion))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Applies an event and increments the version
        /// </summary>
        /// <param name="event">Event to apply</param>
        /// <param name="isCreationEvent">Is this the event as a result of construction of the Aggregate</param>
        protected void ApplyGenericEvent(Event @event, bool isCreationEvent)
        {
            if (this.CanApply(@event, isCreationEvent))
            {
                this.Id = @event.AggregateId; //This is only needed for the very first event as every other event CAN ONLY apply to matching ID
                this.CurrentVersion++;
            }
            else
            {
                throw new AggregateStateMismatchException($"The event target version is {@event.TargetVersion} and AggregateRoot version is {this.CurrentVersion}");
            }
        }
    }
}
