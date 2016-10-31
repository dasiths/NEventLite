using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using EventStore.ClientAPI;

namespace EventSourcingDemo.Storage
{
    class EventstoreEventStorageProvider : IEventStorageProvider
    {
        private static readonly string eventsourcedemo = "EventSourceDemo";

        public IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end)
        {

            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var streamEvents = connection.ReadStreamEventsForwardAsync($"{eventsourcedemo}-{aggregateId}", start - 1, end - 1, false).Result;

            var events = new List<Event>();

            foreach (var returnedEvent in streamEvents.Events)
            {
                var strObjectType = Encoding.UTF8.GetString(returnedEvent.Event.Metadata);
                events.Add((Event)Newtonsoft.Json.JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(returnedEvent.Event.Data)));
            }

            connection.Close();

            return events;
        }

        public void CommitChanges(AggregateRoot aggregate)
        {

            //Connection to the local eventstore on default port 1113
            var connection = GetEventStoreConnection();
            connection.ConnectAsync().Wait();

            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                var ExpectedVersion = aggregate.LastCommittedVersion;

                foreach (var @event in events)
                {
                    var myEvent = new EventData(@event.Id, @event.GetType().ToString().ToLower(), false,
                            Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(@event)),
                            Encoding.UTF8.GetBytes(@event.GetType().ToString()));

                    connection.AppendToStreamAsync($"{eventsourcedemo}-{aggregate.Id}", ExpectedVersion, myEvent).Wait(); //ExpectedVersion.Any

                    ExpectedVersion++;
                }
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
