using System;
using System.Linq;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreSnapshotStorageProvider : EventstoreStorageProviderBase, IEventSnapshotStorageProvider
    {
        public int SnapshotFrequency { get; set; }

        public Snapshot GetSnapshot(Type aggregateType, Guid aggregateId)
        {

            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(aggregateType, aggregateId)}", StreamPosition.End, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = DeserializeSnapshotEvent(result);
            }

            connection.Close();

            return snapshot;
        }

        public void SaveSnapshot(Type aggregateType, Snapshot snapshot)
        {
            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var snapshotyEvent = SerializeSnapshotEvent(snapshot,snapshot.Version);

            connection.AppendToStreamAsync($"{AggregateIdToStreamName(aggregateType, snapshot.AggregateId)}",
                                                ExpectedVersion.Any, snapshotyEvent).Wait();

            connection.Close();
        }
        
        public Snapshot GetSnapshot(Type aggregateType, Guid aggregateId, int version)
        {
            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(aggregateType,aggregateId)}", version, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = DeserializeSnapshotEvent(result);
            }

            connection.Close();

            return snapshot;
        }

        protected abstract IEventStoreConnection GetEventStoreConnection();

    }
}
