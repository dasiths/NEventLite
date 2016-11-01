using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using System;
using System.Collections.Generic;

namespace EventSourcingDemo.Storage
{
    public interface IEventStorageProvider
    {
        IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end);
        void CommitChanges(AggregateRoot aggregate);
        bool HasConcurrencyCheck { get; }
    }
}


