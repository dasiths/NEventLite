using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Exceptions;
using NEventLite.Util;

namespace NEventLite.Core.Domain
{
    public abstract class AggregateRoot<TAggregateKey, TEventKey>
    {
        public TAggregateKey Id { get; private set; }
        public int CurrentVersion { get; private set; }
        public int LastCommittedVersion { get; private set; }
        public StreamState StreamState => (CurrentVersion == -1) ? StreamState.NoStream : StreamState.HasStream;

        private readonly IList<IEvent<TEventKey, TAggregateKey>> _uncommittedChanges = new List<IEvent<TEventKey, TAggregateKey>>();
        private readonly Dictionary<Type, string> _eventHandlerCache;

        protected AggregateRoot()
        {
            CurrentVersion = (int)StreamState.NoStream;
            LastCommittedVersion = (int)StreamState.NoStream;
            _eventHandlerCache = ReflectionHelper.FindEventHandlerMethodsInAggregate<TAggregateKey, TEventKey>(this.GetType());
        }

        public bool HasUncommittedChanges()
        {
            lock (_uncommittedChanges)
            {
                return _uncommittedChanges.Any();
            }
        }

        public ReadOnlyCollection<IEvent<TEventKey, TAggregateKey>> GetUncommittedChanges()
        {
            return new ReadOnlyCollection<IEvent<TEventKey, TAggregateKey>>(_uncommittedChanges);
        }

        public void MarkChangesAsCommitted()
        {
            lock (_uncommittedChanges)
            {
                _uncommittedChanges.Clear();
                LastCommittedVersion = CurrentVersion;
            }
        }

        public async Task LoadsFromHistoryAsync(IEnumerable<IEvent<TEventKey, TAggregateKey>> history)
        {
            foreach (var e in history)
            {
                //We call ApplyEvent with isNew parameter set to false as we are replaying a historical event
                await ApplyEventAsync(e, false);
            }

            LastCommittedVersion = CurrentVersion;
        }

        protected async Task ApplyEventAsync(IEvent<TEventKey, TAggregateKey> @event)
        {
            await ApplyEventAsync(@event, true);
        }

        protected void ApplyEvent(IEvent<TEventKey, TAggregateKey> @event)
        {
            ApplyEventAsync(@event, true).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task ApplyEventAsync(IEvent<TEventKey, TAggregateKey> @event, bool isNew)
        {
            //All state changes to AggregateRoot must happen via the Apply method
            //Make sure the right Apply method is called with the right type.
            //We can you use dynamic objects or reflection for this.

            if (CanApply(@event))
            {
                await DoApplyAsync(@event);

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

        private bool CanApply(IEvent<TEventKey, TAggregateKey> @event)
        {
            //Check to see if event is applying against the right Aggregate and matches the target version
            if (((StreamState == StreamState.NoStream) || (Id.Equals(@event.AggregateId))) && (CurrentVersion == @event.TargetVersion))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task DoApplyAsync(IEvent<TEventKey, TAggregateKey> @event)
        {
            if (StreamState == StreamState.NoStream)
            {
                Id = @event.AggregateId; //This is only needed for the very first event as every other event CAN ONLY apply to matching ID
            }

            if (_eventHandlerCache.ContainsKey(@event.GetType()))
            {
                await @event.InvokeOnAggregateAsync(this, _eventHandlerCache[@event.GetType()]);
            }
            else
            {
                throw new AggregateEventOnApplyMethodMissingException($"No event handler specified for {@event.GetType()} on {this.GetType()}");
            }

            CurrentVersion++;
        }
    }
}
