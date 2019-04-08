using EventStore.ClientAPI;

namespace NEventLite_Example.Storage
{
    class MyEventstoreEventStorageProvider: NEventLite_Storage_Providers.EventStore.EventstoreEventStorageProvider
    {
        protected override IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }

        protected override string GetStreamNamePrefix()
        {
            return Util.EventstoreConnection.GetAggregateStreamPrefix();
        }
    }
}
