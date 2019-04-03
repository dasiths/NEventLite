using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider<TSnapshot> : IEventSnapshotStorageProvider<TSnapshot, Guid, Guid> where TSnapshot : ISnapshot<Guid, Guid>
    {
    }

    public interface IEventSnapshotStorageProvider<TSnapshot, in TAggregateKey, TSnapshotKey> : ISnapshotStorageProvider<TSnapshot, TAggregateKey, TSnapshotKey> 
        where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>
    {
        Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId, int version);
    }
}