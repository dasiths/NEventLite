using System;
using EventSourcingDemo.Domain;

namespace EventSourcingDemo.Events
{
    [Serializable]
    public class Event : IEvent
    {
        public int TargetVersion { get; set; }
        public Guid AggregateId { get;  set; }
        public Guid Id { get; set; }

        protected void Setup(Guid AggregateID, int TargetVersion)
        {
            this.AggregateId = AggregateID;
            this.TargetVersion = TargetVersion;
            this.Id = Guid.NewGuid();
        }
    }

}
