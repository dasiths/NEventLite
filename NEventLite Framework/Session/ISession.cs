using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Storage;

namespace NEventLite.Session
{
    public interface ISession
    {
        IEventStorageProvider EventStorageProvider { get; }
        ISnapshotStorageProvider SnapshotStorageProvider { get; }
        void Add(AggregateRoot aggregate);
        bool IsTracked(Guid id);

        void HandlePreCommited();
        void CommitChanges();
        void HandlePostCommited(IEnumerable<IEvent> events);
    }
}
