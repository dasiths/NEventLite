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

        public IEnumerable<Event> GetEvents<T>(Guid aggregateId, int start, int count) where T : AggregateRoot
        {

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = ReadEvents<T>(connection, aggregateId, start, count);

            connection.Close();

            return events;
        }

        protected IEnumerable<Event> ReadEvents<T>(IEventStoreConnection connection, Guid aggregateId, int start, int count) where T : AggregateRoot
        {

            var events = new List<Event>();
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            var nextSliceStart = start < 0 ? StreamPosition.Start: start;

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

                currentSlice = connection.ReadStreamEventsForwardAsync($"{AggregateIdToStreamName(typeof(T),aggregateId)}", nextSliceStart, nextReadCount, false).Result;
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

        public void CommitChanges<T>(T aggregate) where T : AggregateRoot
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

                connection.AppendToStreamAsync($"{AggregateIdToStreamName(aggregate.GetType(),aggregate.Id)}",
                                                (LastVersion < 0 ? ExpectedVersion.Any : LastVersion), lstEventData).Wait();
            }

            connection.Close();
        }

        protected abstract IEventStoreConnection GetEventStoreConnection();

    }
}
