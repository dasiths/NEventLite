using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider<TAggregate, TAggregateKey, TEventKey> where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> GetEventsAsync(TAggregateKey aggregateId, int start, int count);
        Task<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> GetLastEventAsync(TAggregateKey aggregateId);
        Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate);
    }
}