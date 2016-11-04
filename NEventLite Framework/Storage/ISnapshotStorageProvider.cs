using System;
using NEventLite.Domain;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot<T>(Guid aggregateId) where T : AggregateRoot;
        void SaveSnapshot<T>(Snapshot.Snapshot snapshot) where T : AggregateRoot;
    }
}
