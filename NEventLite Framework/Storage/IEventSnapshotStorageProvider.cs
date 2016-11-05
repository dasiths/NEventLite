using System;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider:ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Type aggregateType, Guid aggregateId, int version);
    }
}
