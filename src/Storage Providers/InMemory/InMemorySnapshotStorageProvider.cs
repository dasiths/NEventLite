using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Storage;
using NEventLite_Storage_Providers.Util;

namespace NEventLite_Storage_Providers.InMemory
{
    public class InMemorySnapshotStorageProvider:ISnapshotStorageProvider{

        private readonly Dictionary<Guid,NEventLite.Snapshot.Snapshot> _items = new Dictionary<Guid,NEventLite.Snapshot.Snapshot>();

        private readonly string _memoryDumpFile;
        public int SnapshotFrequency { get; }

        public InMemorySnapshotStorageProvider(int frequency, string memoryDumpFile)
        {
            SnapshotFrequency = frequency;
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _items = SerializerHelper.LoadListFromFile<Dictionary<Guid, NEventLite.Snapshot.Snapshot>>(_memoryDumpFile).First();
            }
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

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] { _items });
        }
    }
}
