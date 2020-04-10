using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NEventLite.StorageProviders.EventStore.Core
{
    public class EventStoreStorageConnectionProvider : IEventStoreStorageConnectionProvider, IDisposable
    {
        private readonly Lazy<Task<IEventStoreConnection>> _lazyConnection;

        public EventStoreStorageConnectionProvider() : this(() => EventStoreConnection
             .Create(new IPEndPoint(IPAddress.Loopback, 1113)))
        {
        }

        public EventStoreStorageConnectionProvider(Func<IEventStoreConnection> eventStoreConnectionFactoryMethod)
        {
            _lazyConnection = new Lazy<Task<IEventStoreConnection>>(async () =>
            {
                var connection = eventStoreConnectionFactoryMethod();
                await connection.ConnectAsync();
                return connection;
            });
        }

        public async Task<IEventStoreConnection> ConnectAsync()
        {
            return await _lazyConnection.Value;
        }

        public void Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                var connection = _lazyConnection.Value.ConfigureAwait(false).GetAwaiter().GetResult();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
