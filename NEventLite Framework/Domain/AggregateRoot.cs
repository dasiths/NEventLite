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
using System.Linq;
using NEventLite.Events;
using NEventLite.Exceptions;
using NEventLite.Extensions;

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

        private readonly List<IEvent> _uncommittedChanges;
        private Dictionary<Type, string> _eventHandlerCache;

        /// <summary>
        /// Aggregates unique Guid
        /// </summary>
        public Guid Id { get; protected set; }
        /// <summary>
        /// Current version of the Aggregate. Starts with -1 and parameterized constructor increments it by 1.
        /// All events will increment this by 1 when Applied.
        /// </summary>
        public int CurrentVersion { get; protected set; }
        /// <summary>
        /// This is the CurrentVersion of the Aggregate when it was saved last. This is used to ensure optimistic concurrency. 
        /// </summary>
        public int LastCommittedVersion { get; protected set; }

        /// <summary>
        /// Get the current state of this Aggregate
        /// </summary>
        /// <returns>StreamState</returns>
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

        /// <summary>
        /// This bootstraps the Aggregate and marks it ready to receive events
        /// </summary>
        protected AggregateRoot()
        {
            CurrentVersion = (int)StreamState.NoStream;
            LastCommittedVersion = (int)StreamState.NoStream;
            _uncommittedChanges = new List<IEvent>();
            SetupEventHandlers();
        }

        /// <summary>
        /// Checks if there are any uncommitted changes
        /// </summary>
        /// <returns></returns>
        public bool HasUncommittedChanges()
        {
            lock (_uncommittedChanges)
            {
                return _uncommittedChanges.Any();
            }
        }

        /// <summary>
        /// Get the events that have been applied but not commited to storage
        /// </summary>
        /// <returns>The uncommited events</returns>
        public IEnumerable<IEvent> GetUncommittedChanges()
        {
            lock (_uncommittedChanges)
            {
                return _uncommittedChanges.ToList();
            }
        }

        /// <summary>
        /// This will mark all the new events as comitted to storage
        /// </summary>
        public void MarkChangesAsCommitted()
        {
            lock (_uncommittedChanges)
            {
                _uncommittedChanges.Clear();
                LastCommittedVersion = CurrentVersion;
            }
        }

        /// <summary>
        /// This will reapply all the events
        /// </summary>
        /// <param name="history">The events to replay</param>
        public void LoadsFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history)
            {
                //We call ApplyEvent with isNew parameter set to false as we are replaying a historical event
                ApplyEvent(e, false);
            }
            LastCommittedVersion = CurrentVersion;
        }

        /// <summary>
        /// This is used to handle new events
        /// </summary>
        /// <param name="event"></param>
        protected void ApplyEvent(IEvent @event)
        {
            ApplyEvent(@event, true);
        }

        /// <summary>
        /// Finds the correct Apply() method in the AggregateRoot and call it to apply changes to state
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="isNew">Is this a new Event</param>
        private void ApplyEvent(IEvent @event, bool isNew)
        {
            //All state changes to AggregateRoot must happen via the Apply method
            //Make sure the right Apply method is called with the right type.
            //We can you use dynamic objects or reflection for this.

            if (CanApply(@event))
            {
                DoApply(@event);

                if (isNew)
                {
                    lock (_uncommittedChanges)
                    {
                        _uncommittedChanges.Add(@event); //only add to the events collection if it's a new event
                    }
                }
            }
            else
            {
                throw new AggregateStateMismatchException(
                    $"The event target version is {@event.AggregateId}.(Version {@event.TargetVersion}) and " +
                    $"AggregateRoot version is {this.Id}.(Version {CurrentVersion})");
            }

        }

        /// <summary>
        /// Determine if the current event can be applied
        /// </summary>
        /// <param name="event">Event to apply</param>
        /// <returns></returns>
        private bool CanApply(IEvent @event)
        {
            //Check to see if event is applying against the right Aggregate and matches the target version
            if (((GetStreamState() == StreamState.NoStream) || (Id == @event.AggregateId)) && (CurrentVersion == @event.TargetVersion))
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
        private void DoApply(IEvent @event)
        {
            if (GetStreamState() == StreamState.NoStream)
            {
                Id = @event.AggregateId; //This is only needed for the very first event as every other event CAN ONLY apply to matching ID
            }

            if (_eventHandlerCache.ContainsKey(@event.GetType()))
            {
                @event.InvokeOnAggregate(this, _eventHandlerCache[@event.GetType()]);
            }
            else
            {
                throw new EventHandlerApplyMethodMissingException($"No event handler specified for {@event.GetType()} on {this.GetType()}");
            }

            CurrentVersion++;
        }

        /// <summary>
        /// This will wireup the event handling methods to corresponding events
        /// </summary>
        private void SetupEventHandlers()
        {
            _eventHandlerCache = ReflectionHelper.FindEventHandlerMethodsInAggregate(this.GetType());
        }
    }
}
