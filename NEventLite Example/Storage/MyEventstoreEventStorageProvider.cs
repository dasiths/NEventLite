using EventStore.ClientAPI;

namespace NEventLite_Example.Storage
{
    class MyEventstoreEventStorageProvider: NEventLite_Storage_Providers.EventStore.EventstoreEventStorageProvider
    {
        public override IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }

        public override string GetStreamNamePrefix()
        {
            return Util.EventstoreConnection.GetAggregateStreamPrefix();
        }
    }
}
