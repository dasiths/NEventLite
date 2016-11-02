using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace EventSourcingDemo.Storage
{
    class EventstoreEventStorageProvider : IEventStorageProvider
    {
        private static readonly string eventNamePrefix = "EventSourceDemo";

        //There is a max limit of 4096 messages per read in eventstore so use paging
        private const int eventStorePageSize = 200;

        public bool HasConcurrencyCheck => true;

        public IEnumerable<Event> GetEvents(Guid aggregateId, int start, int count)
        {

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = ReadEvents(connection, aggregateId, start, count);

            connection.Close();

            return events;
        }

        public IEnumerable<Event> ReadEvents(IEventStoreConnection connection, Guid aggregateId, int start, int count)
        {

            var events = new List<Event>();
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = StreamPosition.Start;

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            //Read the stream using pagesize which was set before.
            //We only need to read the full page ahead if expected results are larger than the page size

            do
            {
                int nextReadCount = count - streamEvents.Count();

                if (nextReadCount > eventStorePageSize)
                {
                    nextReadCount = eventStorePageSize;
                }

                currentSlice = connection.ReadStreamEventsForwardAsync($"{eventNamePrefix}-{aggregateId}", nextSliceStart, nextReadCount, false).Result;
                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);

            } while (!currentSlice.IsEndOfStream);

            foreach (var returnedEvent in streamEvents)
            {
                var strObjectType = Encoding.UTF8.GetString(returnedEvent.Event.Metadata);
                events.Add(JsonConvert.DeserializeObject<Event>(
                    Encoding.UTF8.GetString(returnedEvent.Event.Data), serializerSettings));
            }

            return events;
        }

        public void CommitChanges(AggregateRoot aggregate)
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
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    lstEventData.Add(new EventData(@event.Id, @event.GetType().ToString(), false,
                            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, serializerSettings)),
                            Encoding.UTF8.GetBytes(@event.GetType().ToString())));
                }

                connection.AppendToStreamAsync($"{eventNamePrefix}-{aggregate.Id}",
                                                (LastVersion < 0 ? ExpectedVersion.Any : LastVersion), lstEventData).Wait();
            }

            connection.Close();
        }

        private static IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }
    }
}
