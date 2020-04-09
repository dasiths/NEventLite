using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    // This class is for storing snapshots as a series of events

    public interface IEventSnapshotStorageProvider : IEventSnapshotStorageProvider<Guid, Guid>
    {
    }

    public interface IEventSnapshotStorageProvider<in TAggregateKey, in TSnapshotKey> : ISnapshotStorageProvider<TAggregateKey, TSnapshotKey> 
    {
        Task<TSnapshot> GetSnapshotAsync<TSnapshot>(TAggregateKey aggregateId, int version) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;
    }
}