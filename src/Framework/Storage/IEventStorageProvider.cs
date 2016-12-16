using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Event;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider
    {
        Task<IEnumerable<IEvent>> GetEventsAsync(Type aggregateType, Guid aggregateId, int start, int count);
        Task<IEvent> GetLastEventAsync(Type aggregateType, Guid aggregateId);
        Task CommitChangesAsync(AggregateRoot aggregate);
    }
}


