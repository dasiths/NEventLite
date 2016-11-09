using System.Net;
using EventStore.ClientAPI;
using NEventLite.Logger;

namespace NEventLite_Example.Util
{
    public static class EventstoreConnection
    {
        private static IEventStoreConnection _connection;

        public static IEventStoreConnection GetEventstoreConnection()
        {
           if(_connection==null)
                LogManager.Log("Connecting to EventStore...", LogSeverity.Information);

            if (_connection == null)
            {
                _connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
                _connection.ConnectAsync().Wait();
            }

            return _connection;
        }

        public static void CloseConnection()
        {
            _connection?.Close();

            if (_connection!=null)
                LogManager.Log("Closing Connection to EventStore...", LogSeverity.Information);

            _connection = null;
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
