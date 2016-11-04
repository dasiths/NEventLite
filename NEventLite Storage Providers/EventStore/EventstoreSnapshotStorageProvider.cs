using System;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Domain;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreSnapshotStorageProvider : EventstoreStorageProviderBase, IEventSnapshotStorageProvider
    {
        public Snapshot GetSnapshot<T>(Guid aggregateId) where T : AggregateRoot
        {

            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(typeof(T), aggregateId)}", StreamPosition.End, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = DeserializeSnapshotEvent(result);
            }

            connection.Close();

            return snapshot;
        }

        public void SaveSnapshot<T>(Snapshot snapshot) where T : AggregateRoot
        {
            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var snapshotyEvent = SerializeSnapshotEvent(snapshot,snapshot.Version);

            connection.AppendToStreamAsync($"{AggregateIdToStreamName(typeof(T), snapshot.AggregateId)}",
                                                ExpectedVersion.Any, snapshotyEvent).Wait();

            connection.Close();
        }
        
        public Snapshot GetSnapshot<T>(Guid aggregateId, int version) where T : AggregateRoot
        {
            Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(typeof(T),aggregateId)}", version, 1, false).Result;

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
