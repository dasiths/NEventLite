using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<TSnapshotKey, TAggregateKey>
    {
        int SnapshotFrequency { get; }
        Task<Snapshot<TSnapshotKey,TAggregateKey>> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId);
        Task SaveSnapshotAsync(Type aggregateType, Snapshot<TSnapshotKey, TAggregateKey> snapshot);
    }
}