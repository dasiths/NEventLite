using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.InMemory
{
    public class InMemorySnapshotStorageProvider : 
        InMemorySnapshotStorageProvider<Guid, Guid>, ISnapshotStorageProvider
    {
        public InMemorySnapshotStorageProvider(int frequency, string memoryDumpFile) : base(frequency, memoryDumpFile)
        {
        }
    }

    public class InMemorySnapshotStorageProvider<TAggregateKey, TSnapshotKey> : ISnapshotStorageProvider<TAggregateKey, TSnapshotKey>
    {
        private readonly Dictionary<TAggregateKey, object> _items = new Dictionary<TAggregateKey, object>();

        private readonly string _memoryDumpFile;
        public int SnapshotFrequency { get; }

        public InMemorySnapshotStorageProvider(int frequency) : this(frequency, string.Empty)
        {
        }

        public InMemorySnapshotStorageProvider(int frequency, string memoryDumpFile)
        {
            SnapshotFrequency = frequency;
            _memoryDumpFile = memoryDumpFile;

            if (!string.IsNullOrWhiteSpace(_memoryDumpFile) && File.Exists(_memoryDumpFile))
            {
                _items = SerializerHelper.LoadFromFile(_memoryDumpFile) as Dictionary<TAggregateKey, object> ?? new Dictionary<TAggregateKey, object>();
            }
        }

        public Task<TSnapshot> GetSnapshotAsync<TSnapshot>(TAggregateKey aggregateId) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>
        {
            if (_items.ContainsKey(aggregateId))
            {
                return Task.FromResult((TSnapshot)_items[aggregateId]);
            }

            return Task.FromResult(default(TSnapshot));
        }

        public Task SaveSnapshotAsync<TSnapshot>(TSnapshot snapshot) where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>
        {
            if (_items.ContainsKey(snapshot.AggregateId))
            {
                _items[snapshot.AggregateId] = snapshot;
            }
            else
            {
                _items.Add(snapshot.AggregateId, snapshot);
            }

            if (!string.IsNullOrWhiteSpace(_memoryDumpFile))
            {
                SerializerHelper.SaveToFile(_memoryDumpFile, _items);
            }

            return Task.CompletedTask;
        }
    }
}
