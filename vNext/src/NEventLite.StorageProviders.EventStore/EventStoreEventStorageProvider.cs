using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreEventStorageProvider<TAggregate, TAggregateKey> : 
        EventStoreStorageProviderBase<TAggregate, TAggregateKey>, IEventStorageProvider<TAggregate, TAggregateKey, Guid>
        where TAggregate : AggregateRoot<TAggregateKey, Guid>
    {
        //There is a max limit of 4096 messages per read in eventstore so use paging
        private const int EventStorePageSize = 200;
        private readonly IEventStoreSettings _eventStoreSettings;

        public EventStoreEventStorageProvider(IEventStoreSettings eventStoreSettings)
        {
            _eventStoreSettings = eventStoreSettings;
        }

        private IEventStoreConnection GetEventStoreConnection() => _eventStoreSettings.GetConnection();

        protected override string GetStreamNamePrefix() => _eventStoreSettings.EventStreamPrefix;

        public async Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>>> GetEventsAsync(TAggregateKey aggregateId, int start, int count)
        {
            var connection = GetEventStoreConnection();
            var events = await ReadEvents(typeof(TAggregate), connection, aggregateId, start, count);

            return events;
        }

        public async Task<IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid>> GetLastEventAsync(TAggregateKey aggregateId)
        {
            var connection = GetEventStoreConnection();

            var results = await connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(typeof(TAggregate), aggregateId.ToString())}", StreamPosition.End, 1, false);

            if (results.Status == SliceReadStatus.Success && results.Events.Any())
            {
                return DeserializeEvent(results.Events.First());
            }

            return null;
        }

        public async Task SaveAsync(AggregateRoot<TAggregateKey, Guid> aggregate)
        {
            var connection = GetEventStoreConnection();
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                var lastVersion = aggregate.LastCommittedVersion;
                List<EventData> lstEventData = new List<EventData>();

                foreach (var @event in events)
                {
                    lstEventData.Add(SerializeEvent(@event, aggregate.LastCommittedVersion + 1));
                }

                await connection.AppendToStreamAsync($"{AggregateIdToStreamName(aggregate.GetType(), aggregate.Id.ToString())}",
                    (lastVersion < 0 ? ExpectedVersion.NoStream : lastVersion), lstEventData);
            }
        }

        protected async Task<IEnumerable<IEvent<TAggregate, TAggregateKey, Guid>>> ReadEvents(
            Type aggregateType, IEventStoreConnection connection, TAggregateKey aggregateId, int start, int count)
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

                if (nextReadCount > EventStorePageSize)
                {
                    nextReadCount = EventStorePageSize;
                }

                currentSlice = await connection.ReadStreamEventsForwardAsync(
                    $"{AggregateIdToStreamName(aggregateType, aggregateId.ToString())}", nextSliceStart, nextReadCount, false);

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            //Deserialize and add to events list
            foreach (var returnedEvent in streamEvents)
            {
                events.Add(DeserializeEvent(returnedEvent));
            }

            return events;
        }
    }
}
