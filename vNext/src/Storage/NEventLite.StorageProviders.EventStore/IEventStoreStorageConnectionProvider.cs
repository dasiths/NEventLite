using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace NEventLite.StorageProviders.EventStore
{
    public interface IEventStoreStorageConnectionProvider
    {
        Task<IEventStoreConnection> GetConnectionAsync();
        string EventStreamPrefix { get; }
        string SnapshotStreamPrefix { get; }
        int SnapshotFrequency { get; }
        int PageSize { get; }
    }
}
