using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider<TSnapshotKey, in TAggregateKey, TSnapshot> : ISnapshotStorageProvider<TSnapshotKey, TAggregateKey, TSnapshot> 
        where TSnapshot : ISnapshot<TSnapshotKey, TAggregateKey>
    {
        Task<TSnapshot> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId, int version);
    }
}