using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Storage
{
    public interface IEventStorageProvider
    {
        IEnumerable<Event> GetEvents<T>(Guid aggregateId, int start, int count) where T : AggregateRoot;
        void CommitChanges<T>(T aggregate) where T : AggregateRoot;
        bool HasConcurrencyCheck { get; }
    }
}


