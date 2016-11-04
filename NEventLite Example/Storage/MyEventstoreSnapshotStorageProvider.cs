using EventStore.ClientAPI;

namespace NEventLite_Example.Storage
{
    class MyEventstoreSnapshotStorageProvider:NEventLite_Storage_Providers.EventStore.EventstoreSnapshotStorageProvider
    {
        public override IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }

        protected override string GetStreamNamePrefix()
        {
            return Util.EventstoreConnection.GetSnapshotStreamPrefix();
        }
    }
}
