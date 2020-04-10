namespace NEventLite.StorageProviders.EventStore.Core
{
    public class EventStoreSettings : IEventStoreSettings
    {
        public string EventStreamPrefix { get; }
        public string SnapshotStreamPrefix { get; }
        public int SnapshotFrequency { get; }
        public int PageSize { get; }

        public EventStoreSettings(int snapshotFrequency, int pageSize = 200, string eventStreamPrefix = "Event-", string snapshotStreamPrefix = "Snapshot-")
        {
            EventStreamPrefix = eventStreamPrefix;
            SnapshotStreamPrefix = snapshotStreamPrefix;
            SnapshotFrequency = snapshotFrequency;
            PageSize = pageSize;
        }
    }
}
