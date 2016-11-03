using System;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Guid aggregateId);
        void SaveSnapshot(Snapshot.Snapshot snapshot);
    }
}
