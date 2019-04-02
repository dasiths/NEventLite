using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider<TEventKey, TAggregate, TAggregateKey> where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        Task<IEnumerable<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>> GetEventsAsync(TAggregateKey aggregateId, int start, int count);
        Task<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>> GetLastEventAsync(TAggregateKey aggregateId);
        Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate);
    }
}