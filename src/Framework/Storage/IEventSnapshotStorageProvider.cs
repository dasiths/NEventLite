using System;
using System.Threading.Tasks;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider:ISnapshotStorageProvider
    {
        Task<Snapshot.Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId, int version);
    }
}
