using System;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreSnapshotStorageProvider:ISnapshotStorageProvider
    {
        public NEventLite.Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {

            NEventLite.Snapshot.Snapshot snapshot = null;

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsBackwardAsync(
                $"{GetStreamNamePrefix()}{aggregateId}", StreamPosition.End, 1, false).Result;

            if (streamEvents.Events.Any())
            {
                var result =  streamEvents.Events.FirstOrDefault();

                snapshot = JsonConvert.DeserializeObject<NEventLite.Snapshot.Snapshot>(
                    Encoding.UTF8.GetString(result.Event.Data), serializerSettings);
            }
            
            connection.Close();

            return snapshot;
        }

        public void SaveSnapshot(NEventLite.Snapshot.Snapshot snapshot)
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

            connection.AppendToStreamAsync($"{GetStreamNamePrefix()}{snapshot.AggregateId}",
                                                ExpectedVersion.Any, snapshotyEvent).Wait();

            connection.Close();
        }

        public abstract IEventStoreConnection GetEventStoreConnection();

        public abstract String GetStreamNamePrefix();
    }
}
