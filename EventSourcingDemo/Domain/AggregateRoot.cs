using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Events;

namespace EventSourcingDemo.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Events.Event> _changes;

        public Guid Id { get; internal set; }
        public int Version { get; internal set; }

        protected AggregateRoot()
        {
            Version = 0;
            _changes = new List<Events.Event>();
        }

        public IEnumerable<Events.Event> GetUncommittedChanges()
        {
            return _changes.ToList();
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Events.Event> history)
        {
            foreach (var e in history)
            {
                HandleEvent(e, false);
            }
        }

        protected void HandleEvent(Events.Event @event)
        {
            HandleEvent(@event, true);
        }

        private void HandleEvent(Events.Event @event, bool isNew)
        {
            //All state changes to AggregateRoot must happen via the Apply method
            //Make sure the right Apply method is called with the right type.
            //We will use reflection for this.

            object[] args = new object[] { @event };
            var method = ((object)this).GetType().GetMethod("Apply",new Type[] { @event.GetType() });
            method.Invoke(this, args);

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        private bool CanApply(Event @event, bool isCreationEvent)
        {
            if (((isCreationEvent) || (this.Id == @event.AggregateId)) && (this.Version == @event.TargetVersion))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ApplyGenericEvent(Event @event, bool isCreationEvent)
        {
            if (this.CanApply(@event, isCreationEvent))
            {
                this.Id = @event.AggregateId;
                this.Version++;
            }
            else
            {
                throw new Exception($"The event target version is {@event.TargetVersion} and AggregateRoot version is {this.Version}");
            }
        }
    }
}
