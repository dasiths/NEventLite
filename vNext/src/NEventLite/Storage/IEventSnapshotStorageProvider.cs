using System;
using System.Threading.Tasks;
using NEventLite.Core;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider<TSnapshot, TSnapshotKey, in TAggregateKey> : ISnapshotStorageProvider<TSnapshot, TSnapshotKey, TAggregateKey> 
        where TSnapshot : ISnapshot<TSnapshotKey, TAggregateKey>
    {
        Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId, int version);
    }
}