using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreEventStorageProvider : EventstoreStorageProviderBase, IEventStorageProvider
    {
        //There is a max limit of 4096 messages per read in eventstore so use paging
        private const int eventStorePageSize = 200;

        public bool HasConcurrencyCheck => true;

        public IEnumerable<Event> GetEvents(Type aggregateType, Guid aggregateId, int start, int count)
        {

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = ReadEvents(aggregateType,connection, aggregateId, start, count);

            connection.Close();

            return events;
        }

        protected IEnumerable<Event> ReadEvents(Type aggregateType, IEventStoreConnection connection, Guid aggregateId, int start, int count)
        {

            var events = new List<Event>();
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = start < 0 ? StreamPosition.Start : start;

            //Read the stream using pagesize which was set before.
            //We only need to read the full page ahead if expected results are larger than the page size

            do
            {
                int nextReadCount = count - streamEvents.Count();

                if (nextReadCount > eventStorePageSize)
                {
                    nextReadCount = eventStorePageSize;
                }

                currentSlice = connection.ReadStreamEventsForwardAsync($"{AggregateIdToStreamName(aggregateType, aggregateId)}", nextSliceStart, nextReadCount, false).Result;
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

        public Event GetLastEvent(Type aggregateType, Guid aggregateId)
        {
            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var results = connection.ReadStreamEventsBackwardAsync(
                $"{AggregateIdToStreamName(aggregateType, aggregateId)}", StreamPosition.End, 1, false).Result;

            connection.Close();

            if (results.Status == SliceReadStatus.Success && results.Events.Count() > 0)
            {
                return DeserializeEvent(results.Events.First());
            }
            else
            {
                return null;
            }
        }

        public void CommitChanges(Type aggregateType, AggregateRoot aggregate)
        {
            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                var LastVersion = aggregate.LastCommittedVersion;
                List<EventData> lstEventData = new List<EventData>();

                foreach (var @event in events)
                {
                    lstEventData.Add(SerializeEvent(@event, aggregate.LastCommittedVersion + 1));
                }

                connection.AppendToStreamAsync($"{AggregateIdToStreamName(aggregate.GetType(), aggregate.Id)}",
                                                (LastVersion < 0 ? ExpectedVersion.Any : LastVersion), lstEventData).Wait();
            }

            connection.Close();
        }

        protected abstract IEventStoreConnection GetEventStoreConnection();

    }
}
