using System;
using NEventLite.Domain;
using NEventLite.Storage;

namespace NEventLite.Session
{
    public interface ISession
    {
        IEventStorageProvider EventStorageProvider { get; }
        ISnapshotStorageProvider SnapshotStorageProvider { get; }
        void Add(AggregateRoot aggregate);
        bool IsTracked(Guid id);
        void CommitChanges();
    }
}
