using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.InMemory
{
    public class InMemorySnapshotStorageProvider:ISnapshotStorageProvider{

        private readonly Dictionary<Guid,NEventLite.Snapshot.Snapshot> _items = new Dictionary<Guid,NEventLite.Snapshot.Snapshot>();

        public int SnapshotFrequency { get; }
        public InMemorySnapshotStorageProvider(int frequency)
        {
            SnapshotFrequency = frequency;
        }
        public async Task<NEventLite.Snapshot.Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId)
        {
            if (_items.ContainsKey(aggregateId))
            {
                return _items[aggregateId];
            }
            else
            {
                return null;
            }
        }
        public async Task SaveSnapshotAsync(Type aggregateType, NEventLite.Snapshot.Snapshot snapshot) 
        {
            if (_items.ContainsKey(snapshot.AggregateId))
            {
               _items[snapshot.AggregateId] = snapshot;
            }
            else
            {
               _items.Add(snapshot.AggregateId,snapshot);
            }
        }
    }
}
