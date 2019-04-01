using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<TSnapshotKey, in TAggregateKey, TSnapshot> where TSnapshot: ISnapshot<TSnapshotKey, TAggregateKey>
    {
        int SnapshotFrequency { get; }
        Task<TSnapshot> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId);
        Task SaveSnapshotAsync(Type aggregateType, TSnapshot snapshot);
    }
}