using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.InMemory
{
    public class InMemorySnapshotStorageProvider<TSnapshotKey, TAggregateKey> : ISnapshotStorageProvider<TSnapshotKey, TAggregateKey>
    {

        private readonly Dictionary<TAggregateKey, ISnapshot<TSnapshotKey, TAggregateKey>> _items = new Dictionary<TAggregateKey, ISnapshot<TSnapshotKey, TAggregateKey>>();

        private readonly string _memoryDumpFile;
        public int SnapshotFrequency { get; }

        public InMemorySnapshotStorageProvider(int frequency, string memoryDumpFile)
        {
            SnapshotFrequency = frequency;
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _items = SerializerHelper.LoadListFromFile<Dictionary<TAggregateKey, ISnapshot<TSnapshotKey, TAggregateKey>>>(_memoryDumpFile).First();
            }
        }
        public Task<ISnapshot<TSnapshotKey, TAggregateKey>> GetSnapshotAsync(Type aggregateType, TAggregateKey aggregateId)
        {
            if (_items.ContainsKey(aggregateId))
            {
                return Task.FromResult(_items[aggregateId]);
            }

            return Task.FromResult((ISnapshot<TSnapshotKey, TAggregateKey>)null);
        }
        public Task SaveSnapshotAsync(Type aggregateType, ISnapshot<TSnapshotKey, TAggregateKey> snapshot)
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
