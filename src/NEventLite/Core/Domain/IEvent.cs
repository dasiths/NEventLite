using System;

namespace NEventLite.Core.Domain
{
    // IEvent<,,> does not need many generic definitions because they are implemented in the abstract Event class.
    // Unlike IRepository<,> we don't register IEvent<,,> in the DI hence there is no need for convenience generic variants.
    
    public interface IEvent<out TAggregate, TAggregateKey, TEventKey>: IEvent where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        TEventKey Id { get; set; }
        TAggregateKey AggregateId { get; set; }
    }

    public interface IEvent
    {
        long TargetVersion { get; set; }
        DateTimeOffset EventCommittedTimestamp { get; set; }
        int EventSchemaVersion { get; set; }
        string CorrelationId { get; set; }
    }
}
