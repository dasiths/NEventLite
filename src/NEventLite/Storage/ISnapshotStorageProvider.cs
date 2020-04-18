using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<in TSnapshotKey>
    {
        int SnapshotFrequency { get; }

        Task<TSnapshot> GetSnapshotAsync<TSnapshot, TAggregateKey>(TAggregateKey aggregateId)
            where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;

        Task SaveSnapshotAsync<TSnapshot, TAggregateKey>(TSnapshot snapshot)
            where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>;
    }
}