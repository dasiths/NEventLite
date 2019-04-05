using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreSnapshotStorageProvider<TAggregate, TSnapshot, TAggregateKey> : 
        EventStoreStorageProviderBase<TAggregate, TAggregateKey>, 
        ISnapshotStorageProvider<TSnapshot, TAggregateKey, Guid>
        where TAggregate : AggregateRoot<TAggregateKey, Guid> where TSnapshot : ISnapshot<TAggregateKey, Guid>
    {
        private readonly IEventStoreSettings _eventStoreSettings;

        public EventStoreSnapshotStorageProvider(IEventStoreSettings eventStoreSettings)
        {
            _eventStoreSettings = eventStoreSettings;
        }

        private IEventStoreConnection GetEventStoreConnection() => _eventStoreSettings.GetConnection();

        protected override string GetStreamNamePrefix() => _eventStoreSettings.SnapshotStreamPrefix;

        public int SnapshotFrequency => _eventStoreSettings.SnapshotFrequency;

        public async Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId)
        {
            TSnapshot snapshot = default(TSnapshot);

            var connection = GetEventStoreConnection();

            var streamEvents = await connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(typeof(TAggregate), aggregateId.ToString())}", StreamPosition.End, 1, false);

            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();
                snapshot = DeserializeSnapshotEvent<TSnapshot>(result);
            }

            return snapshot;
        }

        public async Task SaveSnapshotAsync(TSnapshot snapshot)
        {
            var connection = GetEventStoreConnection();

            var snapshotEvent = SerializeSnapshotEvent(snapshot, snapshot.Version);

            await connection.AppendToStreamAsync($"{AggregateIdToStreamName(typeof(TAggregate), snapshot.AggregateId.ToString())}",
                ExpectedVersion.Any, snapshotEvent);
        }
    }
}
