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

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(typeof(T), aggregateId)}", StreamPosition.End, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                snapshot = JsonConvert.DeserializeObject<Snapshot>(
                    Encoding.UTF8.GetString(result.Event.Data), serializerSettings);
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

            var snapshotyEvent = new EventData(snapshot.Id, @snapshot.GetType().ToString(), false,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(snapshot, serializerSettings)),
                Encoding.UTF8.GetBytes(snapshot.GetType().ToString()));

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

                snapshot = JsonConvert.DeserializeObject<Snapshot>(
                    Encoding.UTF8.GetString(result.Event.Data), serializerSettings);
            }

            connection.Close();

            return snapshot;
        }

        public abstract IEventStoreConnection GetEventStoreConnection();

    }
}
