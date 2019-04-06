using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NEventLite.StorageProviders.EventStore;

namespace NEventLite.Samples.ConsoleApp
{
    public class EventStoreStorageConnectionProvider : IEventStoreStorageConnectionProvider, IDisposable
    {
        private IEventStoreConnection _connection;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(0, 0);

        public async Task<IEventStoreConnection> GetConnectionAsync()
        {
            try
            {
               if (_connection == null) {
                    _connection = EventStoreConnection
                        .Create(new IPEndPoint(IPAddress.Loopback, 1113));
                    await _connection.ConnectAsync();
               }

                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }

        public string EventStreamPrefix => "Event-";
        public string SnapshotStreamPrefix => "Snapshot-";
        public int SnapshotFrequency => 2;
        public int PageSize => 200;

        public void CloseConnection()
        {
            try
            {
                _lock.Wait();

                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            finally
            {
                _lock.Release();
            }

        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
