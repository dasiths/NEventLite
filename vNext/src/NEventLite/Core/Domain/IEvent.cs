using System;

namespace NEventLite.Core.Domain
{
    public interface IEvent<out TAggregate, TAggregateKey, TEventKey> where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        TEventKey Id { get; set; }
        TAggregateKey AggregateId { get; set; }
        int TargetVersion { get; set; }
        DateTimeOffset EventCommittedTimestamp { get; set; }
        int EventSchemaVersion { get; set; }
        string CorrelationId { get; set; }
    }
}
