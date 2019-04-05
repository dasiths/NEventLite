using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;

namespace NEventLite.StorageProviders.EventStore
{
    public interface IEventStoreSettings
    {
        IEventStoreConnection GetConnection();
        string EventStreamPrefix { get; }
        string SnapshotStreamPrefix { get; }
        int SnapshotFrequency { get; }
    }
}
