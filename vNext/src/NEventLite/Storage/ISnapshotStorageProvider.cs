using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<TSnapshot, TSnapshotKey, in TAggregateKey> where TSnapshot: ISnapshot<TSnapshotKey, TAggregateKey>
    {
        int SnapshotFrequency { get; }
        Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId);
        Task SaveSnapshotAsync(TSnapshot snapshot);
    }
}