using System;
using NEventLite.Domain;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Type aggregateType, Guid aggregateId);
        void SaveSnapshot(Type aggregateType, Snapshot.Snapshot snapshot);
    }
}
