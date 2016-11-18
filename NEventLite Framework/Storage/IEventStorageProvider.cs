using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider
    {
        Task<IEnumerable<IEvent>> GetEvents(Type aggregateType, Guid aggregateId, int start, int count);
        Task<IEvent> GetLastEvent(Type aggregateType, Guid aggregateId);
        Task CommitChanges(AggregateRoot aggregate);
    }
}


