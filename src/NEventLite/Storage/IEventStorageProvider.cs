using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider : IEventStorageProvider<Guid> 
    { 
    }

    public interface IEventStorageProvider<TEventKey>
    {
        Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> GetEventsAsync<TAggregate, TAggregateKey>(TAggregateKey aggregateId, int start, int count) 
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>;

        Task<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> GetLastEventAsync<TAggregate, TAggregateKey>(TAggregateKey aggregateId)
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>;

        Task SaveAsync<TAggregate, TAggregateKey>(TAggregate aggregate) where TAggregate: AggregateRoot<TAggregateKey, TEventKey>;
    }
}