using System;
using System.Threading.Tasks;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider:ISnapshotStorageProvider
    {
        Task<Snapshot.Snapshot> GetSnapshot(Type aggregateType, Guid aggregateId, int version);
    }
}
