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
        private const int maxEventStoreReadCount = 4096;

        public bool HasConcurrencyCheck => true;

        public IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end)
        {

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = ReadEvents(connection, aggregateId, start, end);

            connection.Close();

            return events;
        }

        public IEnumerable<Event> ReadEvents(IEventStoreConnection connection, Guid aggregateId, int start, int end)
        {

            var events = new List<Event>();

            //There is a max limit of 4096 messages per read in eventstore so use paging
            //Todo: Use streamEvents.IsEndOfStream() and streamEvents.NextEventNumber to implement reccomended paging method

            int readUpTo = end;
            if (end - start >= maxEventStoreReadCount)
            {
                end = start + maxEventStoreReadCount;
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var streamEvents = connection.ReadStreamEventsForwardAsync(
                $"{eventNamePrefix}-{aggregateId}", (start == 0 ? 0 : start - 1), (end - start) - 1, false).Result;

            foreach (var returnedEvent in streamEvents.Events)
            {
                var strObjectType = Encoding.UTF8.GetString(returnedEvent.Event.Metadata);
                events.Add(JsonConvert.DeserializeObject<Event>(
                    Encoding.UTF8.GetString(returnedEvent.Event.Data), serializerSettings));
            }

            //recursively call with new start value to load next page
            //No need to try to read again if the last read returned less than the max count
            if ((events.Count() >= (maxEventStoreReadCount - 1)) && (end < readUpTo))
            {
                events.AddRange(ReadEvents(connection, aggregateId, end, readUpTo));
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
                var LastVersion = aggregate.LastCommittedVersion - 1;
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
            //Connection to the local eventstore on default port 1113
            return EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
        }
    }
}
