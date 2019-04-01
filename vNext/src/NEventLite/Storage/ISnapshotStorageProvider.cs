using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface ISnapshotStorageProvider<TSnapshotKey, TAggregateKey>
    {
        int SnapshotFrequency { get; }
        Task<ISnapshot<TSnapshotKey,TAggregateKey>> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId);
        Task SaveSnapshotAsync(Type aggregateType, ISnapshot<TSnapshotKey, TAggregateKey> snapshot);
    }
}