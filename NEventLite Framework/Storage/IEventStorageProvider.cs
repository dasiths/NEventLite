using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider
    {
        IEnumerable<IEvent> GetEvents(Type aggregateType, Guid aggregateId, int start, int count);
        IEvent GetLastEvent(Type aggregateType, Guid aggregateId);
        void CommitChanges(Type aggregateType, AggregateRoot aggregate);
    }
}


