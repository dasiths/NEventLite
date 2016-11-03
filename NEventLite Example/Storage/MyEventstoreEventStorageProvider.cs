using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace EventSourcingDemo.Storage
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
