using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider : IEventStorageProvider<Guid, Guid> 
    { 
    }

    public interface IEventStorageProvider<TAggregateKey, TEventKey>
    {
        Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> GetEventsAsync<TAggregate>(TAggregateKey aggregateId, int start, int count) 
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>;

        Task<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> GetLastEventAsync<TAggregate>(TAggregateKey aggregateId)
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>;

        Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate: AggregateRoot<TAggregateKey, TEventKey>;
    }
}