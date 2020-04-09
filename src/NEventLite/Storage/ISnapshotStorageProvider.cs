using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider : ISnapshotStorageProvider<Guid, Guid>
    {
    }

    public interface ISnapshotStorageProvider<in TAggregateKey, in TSnapshotKey>
    {
        int SnapshotFrequency { get; }
        Task<TSnapshot> GetSnapshotAsync<TSnapshot>(TAggregateKey aggregateId) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;
        Task SaveSnapshotAsync<TSnapshot>(TSnapshot snapshot) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;
    }
}