using System.Net;
using EventStore.ClientAPI;

namespace NEventLite_Example.Util
{
    public static class EventstoreConnection
    {
        public static IEventStoreConnection GetEventstoreConnection()
        {
            //Connection to the local eventstore on default port 1113
            return EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
        }

        public static string GetAggregateStreamPrefix()
        {
            return "EventSourceDemo-";
        }

        public static string GetSnapshotStreamPrefix()
        {
            return "EventSourceDemo-Snapshot-";
        }
    }
}
