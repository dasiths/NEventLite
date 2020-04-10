namespace NEventLite.StorageProviders.EventStore.Core
{
    public interface IEventStoreSettings
    {
        string EventStreamPrefix { get; }
        string SnapshotStreamPrefix { get; }
        int SnapshotFrequency { get; }
        int PageSize { get; }
    }
}