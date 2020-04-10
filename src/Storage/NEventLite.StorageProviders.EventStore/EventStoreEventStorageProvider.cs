using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core.Domain;
using NEventLite.Storage;
using NEventLite.StorageProviders.EventStore.Core;

namespace NEventLite.StorageProviders.EventStore
{

    public class EventStoreEventStorageProvider :  IEventStorageProvider
    {
        private readonly IEventStoreStorageConnectionProvider _eventStoreStorageConnectionProvider;
        private readonly IEventStoreStorageCore _eventStoreStorageCore;
        private readonly IEventStoreSettings _eventStoreSettings;

        public EventStoreEventStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider,
            IEventStoreStorageCore eventStoreStorageCore,
            IEventStoreSettings eventStoreSettings)
        {
            _eventStoreStorageConnectionProvider = eventStoreStorageConnectionProvider;
            _eventStoreStorageCore = eventStoreStorageCore;
            _eventStoreSettings = eventStoreSettings;
        }

        private Task<IEventStoreConnection> GetEventStoreConnectionAsync() => _eventStoreStorageConnectionProvider.ConnectAsync();


        public async Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>>> GetEventsAsync<TAggregate, TAggregateKey>(
            TAggregateKey aggregateId, int start, int count) 
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var events = await ReadEvents<TAggregate, TAggregateKey>(connection, aggregateId, start, count);

            return events;
        }

        public async Task<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>> GetLastEventAsync<TAggregate, TAggregateKey>(TAggregateKey aggregateId) 
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var results = await connection.ReadStreamEventsBackwardAsync(
                $"{_eventStoreStorageCore.TypeToStreamName<TAggregate>(aggregateId.ToString(), _eventStoreSettings.EventStreamPrefix)}", StreamPosition.End, 1, false);

            if (results.Status == SliceReadStatus.Success && results.Events.Any())
            {
                return _eventStoreStorageCore.DeserializeEvent<TAggregate, TAggregateKey>(results.Events.First());
            }

            return null;
        }

        public async Task SaveAsync<TAggregate, TAggregateKey>(TAggregate aggregate) where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                var lastVersion = aggregate.LastCommittedVersion;
                var lstEventData = new List<EventData>();

                foreach (var @event in events)
                {
                    lstEventData.Add(_eventStoreStorageCore.SerializeEvent(@event, aggregate.LastCommittedVersion + 1));
                }

                await connection.AppendToStreamAsync($"{_eventStoreStorageCore.TypeToStreamName<TAggregate>(aggregate.Id.ToString(), _eventStoreSettings.EventStreamPrefix)}",
                    (lastVersion < 0 ? ExpectedVersion.NoStream : lastVersion), lstEventData);
            }
        }

        protected async Task<IEnumerable<IEvent<TAggregate, TAggregateKey, Guid>>> ReadEvents<TAggregate, TAggregateKey>(
            IEventStoreConnection connection, TAggregateKey aggregateId, int start, int count)
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var events = new List<IEvent<TAggregate, TAggregateKey, Guid>>();
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = start < 0 ? StreamPosition.Start : start;

            //Read the stream using pagesize which was set before.
            //We only need to read the full page ahead if expected results are larger than the page size

            do
            {
                var nextReadCount = count - streamEvents.Count();

                if (nextReadCount > _eventStoreSettings.PageSize)
                {
                    nextReadCount = _eventStoreSettings.PageSize;
                }

                currentSlice = await connection.ReadStreamEventsForwardAsync(
                    $"{_eventStoreStorageCore.TypeToStreamName<TAggregate>(aggregateId.ToString(), _eventStoreSettings.EventStreamPrefix)}", nextSliceStart, nextReadCount, false);

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            //Deserialize and add to events list
            foreach (var returnedEvent in streamEvents)
            {
                events.Add(_eventStoreStorageCore.DeserializeEvent<TAggregate, TAggregateKey>(returnedEvent));
            }

            return events;
        }
    }
}
