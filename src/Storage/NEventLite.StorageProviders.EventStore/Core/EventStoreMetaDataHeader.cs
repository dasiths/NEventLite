namespace NEventLite.StorageProviders.EventStore.Core
{
    public class EventStoreMetaDataHeader
    {
        public string ClrType { get; set; }
        public long CommitNumber { get; set; }
    }
}
