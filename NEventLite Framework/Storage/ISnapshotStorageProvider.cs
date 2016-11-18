using System;
using System.Threading.Tasks;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider
    {
        int SnapshotFrequency { get; }
        Task<Snapshot.Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId);
        Task SaveSnapshotAsync(Type aggregateType, Snapshot.Snapshot snapshot);
    }
}
