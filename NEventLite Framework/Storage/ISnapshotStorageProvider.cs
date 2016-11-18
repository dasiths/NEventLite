using System;
using System.Threading.Tasks;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        int SnapshotFrequency { get; }
        Task<Snapshot.Snapshot> GetSnapshot(Type aggregateType, Guid aggregateId);
        Task SaveSnapshot(Type aggregateType, Snapshot.Snapshot snapshot);
    }
}
