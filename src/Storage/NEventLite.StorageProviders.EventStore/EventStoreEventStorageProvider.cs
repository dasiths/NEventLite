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

    public class EventStoreEventStorageProvider : IEventStorageProvider
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
            TAggregateKey aggregateId, long start, long count)
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

        public async Task SaveAsync<TAggregate, TAggregateKey>(TAggregateKey aggregateId, IEnumerable<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>> events) where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connectionTask = GetEventStoreConnectionAsync();
            var changesToCommit = events.ToList();

            var lstEventData = changesToCommit.Select(@event =>
                    _eventStoreStorageCore.SerializeEvent(@event, @event.TargetVersion + 1)).ToList();

            if (lstEventData.Any())
            {
                var lastVersion = changesToCommit.First().TargetVersion;

                var connection = await connectionTask;
                await connection.AppendToStreamAsync($"{_eventStoreStorageCore.TypeToStreamName<TAggregate>(aggregateId.ToString(), _eventStoreSettings.EventStreamPrefix)}",
                    (lastVersion < 0 ? ExpectedVersion.NoStream : lastVersion), lstEventData);
            }
        }

        protected async Task<IEnumerable<IEvent<TAggregate, TAggregateKey, Guid>>> ReadEvents<TAggregate, TAggregateKey>(
            IEventStoreConnection connection, TAggregateKey aggregateId, long start, long count)
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var events = new List<IEvent<TAggregate, TAggregateKey, Guid>>();
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = start < 0 ? StreamPosition.Start : start;

            //Read the stream using pagesize which was set before.
            //We only need to read the full page ahead if expected results are larger than the page size

            do
            {
                var totalNextReadCount = count - streamEvents.Count();

                var nextReadCount = totalNextReadCount > _eventStoreSettings.PageSize ? 
                    _eventStoreSettings.PageSize : Convert.ToInt32(totalNextReadCount);

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
