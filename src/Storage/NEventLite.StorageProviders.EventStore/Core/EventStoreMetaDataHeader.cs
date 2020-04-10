namespace NEventLite.StorageProviders.EventStore.Core
{
    public class EventStoreMetaDataHeader
    {
        public string ClrType { get; set; }
        public int CommitNumber { get; set; }
    }
}
