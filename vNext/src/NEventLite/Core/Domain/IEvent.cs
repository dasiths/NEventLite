using System;

namespace NEventLite.Core.Domain
{
    public interface IEvent<TKey, out TAggregate, TAggregateKey> where TAggregate : AggregateRoot<TAggregateKey, TKey>
    {
        TKey Id { get; set; }
        TAggregateKey AggregateId { get; set; }
        int TargetVersion { get; set; }
        DateTimeOffset EventCommittedTimestamp { get; set; }
        int EventSchemaVersion { get; set; }
        string CorrelationId { get; set; }
    }
}
