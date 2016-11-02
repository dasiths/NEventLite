using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventSourcingDemo.Storage
{
    class EventstoreSnapshotStorageProvider:ISnapshotStorageProvider
    {
        private static readonly string eventNamePrefix = "EventSourceDemo_Snapshot";

        public Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {

            Snapshot.Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{eventNamePrefix}-{aggregateId}", StreamPosition.End, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result =  streamEvents.Events.FirstOrDefault();

                snapshot = JsonConvert.DeserializeObject<Snapshot.Snapshot>(
                    Encoding.UTF8.GetString(result.Event.Data), serializerSettings);
            }
            
            connection.Close();

            return snapshot;
        }

        public void SaveSnapshot(Snapshot.Snapshot snapshot)
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

            connection.AppendToStreamAsync($"{eventNamePrefix}-{snapshot.AggregateId}",
                                                ExpectedVersion.Any, snapshotyEvent).Wait();

            connection.Close();
        }

        private static IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }
    }
}
