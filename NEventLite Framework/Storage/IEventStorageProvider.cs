using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider
    {
        IEnumerable<Event> GetEvents(Guid aggregateId, int start, int count);
        void CommitChanges(AggregateRoot aggregate);
        bool HasConcurrencyCheck { get; }
    }
}


