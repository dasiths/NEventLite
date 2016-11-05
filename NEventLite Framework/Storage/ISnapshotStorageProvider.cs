using System;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        int SnapshotFrequency { get; set; }
        Snapshot.Snapshot GetSnapshot(Type aggregateType, Guid aggregateId);
        void SaveSnapshot(Type aggregateType, Snapshot.Snapshot snapshot);
    }
}
