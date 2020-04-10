using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NEventLite.StorageProviders.EventStore.Core
{
    public interface IEventStoreStorageConnectionProvider
    {
        Task<IEventStoreConnection> ConnectAsync();
    }
}
