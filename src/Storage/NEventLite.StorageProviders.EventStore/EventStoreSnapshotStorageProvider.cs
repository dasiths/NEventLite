using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;
using NEventLite.StorageProviders.EventStore.Core;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreSnapshotStorageProvider : ISnapshotStorageProvider<Guid>
    {
        private readonly IEventStoreStorageConnectionProvider _eventStoreStorageConnectionProvider;
        private readonly IEventStoreStorageCore _eventStoreStorageCore;
        private readonly IEventStoreSettings _eventStoreSettings;

        public EventStoreSnapshotStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider, 
            IEventStoreStorageCore eventStoreStorageCore, 
            IEventStoreSettings eventStoreSettings)
        {
            _eventStoreStorageConnectionProvider = eventStoreStorageConnectionProvider;
            _eventStoreStorageCore = eventStoreStorageCore;
            _eventStoreSettings = eventStoreSettings;
        }

        private Task<IEventStoreConnection> GetEventStoreConnectionAsync() => _eventStoreStorageConnectionProvider.ConnectAsync();

        public int SnapshotFrequency => _eventStoreSettings.SnapshotFrequency;

        public async Task<TSnapshot> GetSnapshotAsync<TSnapshot, TAggregateKey>(TAggregateKey aggregateId) where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            var snapshot = default(TSnapshot);
            var connection = await GetEventStoreConnectionAsync();

            var streamEvents = await connection.ReadStreamEventsBackwardAsync(
                $"{_eventStoreStorageCore.TypeToStreamName<TSnapshot>(aggregateId.ToString(), _eventStoreSettings.SnapshotStreamPrefix)}", StreamPosition.End, 1, false);

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();
                snapshot = _eventStoreStorageCore.DeserializeSnapshotEvent<TSnapshot>(result);
            }

            return snapshot;
        }

        public async Task SaveSnapshotAsync<TSnapshot, TAggregateKey>(TSnapshot snapshot) where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var snapshotEvent = _eventStoreStorageCore.SerializeSnapshotEvent<TSnapshot, TAggregateKey>(snapshot, snapshot.Version);

            await connection.AppendToStreamAsync($"{_eventStoreStorageCore.TypeToStreamName<TSnapshot>(snapshot.AggregateId.ToString(), _eventStoreSettings.SnapshotStreamPrefix)}",
                ExpectedVersion.Any, snapshotEvent);
        }
    }
}
