using System;

namespace NEventLite.Core.Domain
{
    public abstract class Event<TKey, TAggregate, TAggregateKey> : IEvent<TKey, TAggregate, TAggregateKey> where TAggregate : AggregateRoot<TAggregateKey, TKey>
    {
        protected Event(TKey id, TAggregateKey aggregateId) : this(id, aggregateId, (int) StreamState.NoStream)
        {
        }

        protected Event(TKey id, TAggregateKey aggregateId, int targetVersion) : this(id, aggregateId, targetVersion, 0,
            string.Empty)
        {
        }

        protected Event(TKey id, TAggregateKey aggregateId, int targetVersion, int eventSchemaVersion, string correlationId)
        {
            this.Id = id;
            this.AggregateId = aggregateId;
            this.TargetVersion = targetVersion;
            this.EventSchemaVersion = eventSchemaVersion;
            this.CorrelationId = correlationId;
        }

        public TKey Id { get; set; }
        public TAggregateKey AggregateId { get; set;  }
        public int TargetVersion { get; set;  }
        public DateTimeOffset EventCommittedTimestamp { get; set; }
        public int EventSchemaVersion { get; set; }
        public string CorrelationId { get; set; }
    }
}