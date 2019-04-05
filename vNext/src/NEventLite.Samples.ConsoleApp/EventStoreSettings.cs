using System;
using System.Net;
using EventStore.ClientAPI;
using NEventLite.StorageProviders.EventStore;

namespace NEventLite.Samples.ConsoleApp
{
    public class EventStoreSettings: IEventStoreSettings, IDisposable
    {
        private static IEventStoreConnection _connection;

        public IEventStoreConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
                _connection.ConnectAsync().Wait();
            }

            return _connection;
        }

        public string EventStreamPrefix => "Events";
        public string SnapshotStreamPrefix => "Snapshots";
        public int SnapshotFrequency => 2;

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
