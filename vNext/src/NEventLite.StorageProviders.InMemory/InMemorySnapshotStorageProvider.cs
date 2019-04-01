using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.InMemory
{
    public class InMemorySnapshotStorageProvider<TSnapshotKey, TAggregateKey, TSnapshot> : ISnapshotStorageProvider<TSnapshotKey, TAggregateKey, TSnapshot> 
        where TSnapshot : ISnapshot<TSnapshotKey, TAggregateKey>
    {

        private readonly Dictionary<TAggregateKey, TSnapshot> _items = new Dictionary<TAggregateKey, TSnapshot>();

        private readonly string _memoryDumpFile;
        public int SnapshotFrequency { get; }

        public InMemorySnapshotStorageProvider(int frequency, string memoryDumpFile)
        {
            SnapshotFrequency = frequency;
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _items = SerializerHelper.LoadListFromFile<Dictionary<TAggregateKey, TSnapshot>>(_memoryDumpFile).First();
            }
        }
        public Task<TSnapshot> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId)
        {
            if (_items.ContainsKey(aggregateId))
            {
                return Task.FromResult(_items[aggregateId]);
            }

            return Task.FromResult(default(TSnapshot));
        }
        public Task SaveSnapshotAsync(Type aggregateType, TSnapshot snapshot)
        {
            if (_items.ContainsKey(snapshot.AggregateId))
            {
                _items[snapshot.AggregateId] = snapshot;
            }
            else
            {
                _items.Add(snapshot.AggregateId, snapshot);
            }

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] { _items });

            return Task.CompletedTask;
        }
    }
}
