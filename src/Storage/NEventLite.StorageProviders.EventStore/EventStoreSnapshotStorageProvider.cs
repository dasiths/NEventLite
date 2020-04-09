using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreSnapshotStorageProvider : 
        EventStoreSnapshotStorageProvider<Guid>,
        ISnapshotStorageProvider
    {
        public EventStoreSnapshotStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider) : base(eventStoreStorageConnectionProvider)
        {
        }
    }

    public class EventStoreSnapshotStorageProvider<TAggregateKey> : 
        EventStoreStorageProviderBase<TAggregateKey>, 
        ISnapshotStorageProvider<TAggregateKey, Guid>
    {
        private readonly IEventStoreStorageConnectionProvider _eventStoreStorageConnectionProvider;

        public EventStoreSnapshotStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider)
        {
            _eventStoreStorageConnectionProvider = eventStoreStorageConnectionProvider;
        }

        private Task<IEventStoreConnection> GetEventStoreConnectionAsync() => _eventStoreStorageConnectionProvider.GetConnectionAsync();

        protected override string GetStreamNamePrefix() => _eventStoreStorageConnectionProvider.SnapshotStreamPrefix;

        public int SnapshotFrequency => _eventStoreStorageConnectionProvider.SnapshotFrequency;

        public async Task<TSnapshot> GetSnapshotAsync<TSnapshot>(TAggregateKey aggregateId) where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            TSnapshot snapshot = default(TSnapshot);
            var connection = await GetEventStoreConnectionAsync();

            var streamEvents = await connection.ReadStreamEventsBackwardAsync(
                $"{TypeToStreamName<TSnapshot>(aggregateId.ToString())}", StreamPosition.End, 1, false);

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();
                snapshot = DeserializeSnapshotEvent<TSnapshot>(result);
            }

            return snapshot;
        }

        public async Task SaveSnapshotAsync<TSnapshot>(TSnapshot snapshot) where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var snapshotEvent = SerializeSnapshotEvent(snapshot, snapshot.Version);

            await connection.AppendToStreamAsync($"{TypeToStreamName<TSnapshot>(snapshot.AggregateId.ToString())}",
                ExpectedVersion.Any, snapshotEvent);
        }
    }
}
