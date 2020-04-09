using System;

namespace NEventLite.Core.Domain
{
    public interface IEvent<out TAggregate> : IEvent<TAggregate, Guid, Guid> where TAggregate : AggregateRoot<Guid, Guid>
    {
    }

    public interface IEvent<out TAggregate, TAggregateKey, TEventKey>: IEvent where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        TEventKey Id { get; set; }
        TAggregateKey AggregateId { get; set; }
    }

    public interface IEvent
    {
        int TargetVersion { get; set; }
        DateTimeOffset EventCommittedTimestamp { get; set; }
        int EventSchemaVersion { get; set; }
        string CorrelationId { get; set; }
    }
}
