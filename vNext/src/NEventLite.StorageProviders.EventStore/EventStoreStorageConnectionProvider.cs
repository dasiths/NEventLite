using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreStorageConnectionProvider : IEventStoreStorageConnectionProvider, IDisposable
    {
        private IEventStoreConnection _connection;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        public delegate IEventStoreConnection EventStoreConnectionFactoryMethod();
        private readonly EventStoreConnectionFactoryMethod _eventStoreConnectionFactoryMethod;

        public EventStoreStorageConnectionProvider()
        {
            _eventStoreConnectionFactoryMethod = () => EventStoreConnection
                .Create(new IPEndPoint(IPAddress.Loopback, 1113));
        }

        public EventStoreStorageConnectionProvider(EventStoreConnectionFactoryMethod eventStoreConnectionFactoryMethod)
        {
            _eventStoreConnectionFactoryMethod = eventStoreConnectionFactoryMethod;
        }

        public async Task<IEventStoreConnection> GetConnectionAsync()
        {
            await _lock.WaitAsync();

            try
            {
               if (_connection == null)
               {
                   _connection = _eventStoreConnectionFactoryMethod();
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
            _lock.Wait();

            try
            {
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
