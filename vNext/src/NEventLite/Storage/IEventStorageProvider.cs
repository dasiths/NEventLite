using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider<TEventKey, TAggregateKey>
    {
        Task<IEnumerable<IEvent<TEventKey, TAggregateKey>>> GetEventsAsync(Type aggregateType, TAggregateKey aggregateId, int start, int count);
        Task<IEvent<TEventKey, TAggregateKey>> GetLastEventAsync(Type aggregateType, TAggregateKey aggregateId);
        Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate);
    }
}