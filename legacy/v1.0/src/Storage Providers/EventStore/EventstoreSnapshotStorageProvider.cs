using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreSnapshotStorageProvider : EventstoreStorageProviderBase, IEventSnapshotStorageProvider
    {
        public int SnapshotFrequency { get; }
        protected EventstoreSnapshotStorageProvider(int frequency)
        {
            SnapshotFrequency = frequency;
        }
        public async Task<Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId)
        {

            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();

            var streamEvents = await connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(aggregateType, aggregateId)}", StreamPosition.End, 1, false);

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = DeserializeSnapshotEvent(result);
            }

            return snapshot;
        }
        public async Task SaveSnapshotAsync(Type aggregateType, Snapshot snapshot)
        {
            var connection = GetEventStoreConnection();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var snapshotyEvent = SerializeSnapshotEvent(snapshot,snapshot.Version);

            await connection.AppendToStreamAsync($"{AggregateIdToStreamName(aggregateType, snapshot.AggregateId)}",
                                                ExpectedVersion.Any, snapshotyEvent);
        }
        public async Task<Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId, int version)
        {
            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = await connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(aggregateType,aggregateId)}", version, 1, false);

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = DeserializeSnapshotEvent(result);
            }

            return snapshot;
        }
        protected abstract IEventStoreConnection GetEventStoreConnection();

    }
}
