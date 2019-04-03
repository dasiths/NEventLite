using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<TSnapshot, in TAggregateKey, TSnapshotKey> where TSnapshot: ISnapshot<TAggregateKey, TSnapshotKey>
    {
        int SnapshotFrequency { get; }
        Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId);
        Task SaveSnapshotAsync(TSnapshot snapshot);
    }
}