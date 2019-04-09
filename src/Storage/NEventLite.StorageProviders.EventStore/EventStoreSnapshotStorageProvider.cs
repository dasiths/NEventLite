using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreSnapshotStorageProvider<TAggregate, TSnapshot> : 
        EventStoreSnapshotStorageProvider<TAggregate, TSnapshot, Guid>,
        ISnapshotStorageProvider<TSnapshot>
        where TAggregate : AggregateRoot<Guid, Guid> 
        where TSnapshot : ISnapshot<Guid, Guid>
    {
        public EventStoreSnapshotStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider) : base(eventStoreStorageConnectionProvider)
        {
        }
    }

    public class EventStoreSnapshotStorageProvider<TAggregate, TSnapshot, TAggregateKey> : 
        EventStoreStorageProviderBase<TAggregate, TAggregateKey>, 
        ISnapshotStorageProvider<TSnapshot, TAggregateKey, Guid>
        where TAggregate : AggregateRoot<TAggregateKey, Guid> where TSnapshot : ISnapshot<TAggregateKey, Guid>
    {
        private readonly IEventStoreStorageConnectionProvider _eventStoreStorageConnectionProvider;

        public EventStoreSnapshotStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider)
        {
            _eventStoreStorageConnectionProvider = eventStoreStorageConnectionProvider;
        }

        private Task<IEventStoreConnection> GetEventStoreConnectionAsync() => _eventStoreStorageConnectionProvider.GetConnectionAsync();

        protected override string GetStreamNamePrefix() => _eventStoreStorageConnectionProvider.SnapshotStreamPrefix;

        public int SnapshotFrequency => _eventStoreStorageConnectionProvider.SnapshotFrequency;

        public async Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId)
        {
            TSnapshot snapshot = default(TSnapshot);
            var connection = await GetEventStoreConnectionAsync();

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
            var connection = await GetEventStoreConnectionAsync();
            var snapshotEvent = SerializeSnapshotEvent(snapshot, snapshot.Version);

            await connection.AppendToStreamAsync($"{AggregateIdToStreamName(typeof(TAggregate), snapshot.AggregateId.ToString())}",
                ExpectedVersion.Any, snapshotEvent);
        }
    }
}
