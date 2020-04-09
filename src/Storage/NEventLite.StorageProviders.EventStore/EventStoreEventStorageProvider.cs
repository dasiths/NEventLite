using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreEventStorageProvider :
        EventStoreEventStorageProvider<Guid>,
        IEventStorageProvider
    {
        public EventStoreEventStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider) : base(eventStoreStorageConnectionProvider)
        {
        }
    }

    public class EventStoreEventStorageProvider<TAggregateKey> : 
        EventStoreStorageProviderBase<TAggregateKey>, IEventStorageProvider<TAggregateKey, Guid>
    {
        private readonly IEventStoreStorageConnectionProvider _eventStoreStorageConnectionProvider;

        public EventStoreEventStorageProvider(IEventStoreStorageConnectionProvider eventStoreStorageConnectionProvider)
        {
            _eventStoreStorageConnectionProvider = eventStoreStorageConnectionProvider;
        }

        private Task<IEventStoreConnection> GetEventStoreConnectionAsync() => _eventStoreStorageConnectionProvider.GetConnectionAsync();

        protected override string GetStreamNamePrefix() => _eventStoreStorageConnectionProvider.EventStreamPrefix;

        public async Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>>> GetEventsAsync<TAggregate>(TAggregateKey aggregateId, int start, int count) 
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var events = await ReadEvents<TAggregate>(connection, aggregateId, start, count);

            return events;
        }

        public async Task<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>> GetLastEventAsync<TAggregate>(TAggregateKey aggregateId) 
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var results = await connection.ReadStreamEventsBackwardAsync(
                $"{TypeToStreamName<TAggregate>(aggregateId.ToString())}", StreamPosition.End, 1, false);

            if (results.Status == SliceReadStatus.Success && results.Events.Any())
            {
                return DeserializeEvent<TAggregate>(results.Events.First());
            }

            return null;
        }

        public async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var connection = await GetEventStoreConnectionAsync();
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                var lastVersion = aggregate.LastCommittedVersion;
                List<EventData> lstEventData = new List<EventData>();

                foreach (var @event in events)
                {
                    lstEventData.Add(SerializeEvent(@event, aggregate.LastCommittedVersion + 1));
                }

                await connection.AppendToStreamAsync($"{TypeToStreamName<TAggregate>(aggregate.Id.ToString())}",
                    (lastVersion < 0 ? ExpectedVersion.NoStream : lastVersion), lstEventData);
            }
        }

        protected async Task<IEnumerable<IEvent<TAggregate, TAggregateKey, Guid>>> ReadEvents<TAggregate>(IEventStoreConnection connection, TAggregateKey aggregateId, int start, int count)
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

                if (nextReadCount > _eventStoreStorageConnectionProvider.PageSize)
                {
                    nextReadCount = _eventStoreStorageConnectionProvider.PageSize;
                }

                currentSlice = await connection.ReadStreamEventsForwardAsync(
                    $"{TypeToStreamName<TAggregate>(aggregateId.ToString())}", nextSliceStart, nextReadCount, false);

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            //Deserialize and add to events list
            foreach (var returnedEvent in streamEvents)
            {
                events.Add(DeserializeEvent<TAggregate>(returnedEvent));
            }

            return events;
        }
    }
}
