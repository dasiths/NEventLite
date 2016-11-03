using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace EventSourcingDemo.Storage
{
    class MyEventstoreSnapshotStorageProvider:NEventLite_Storage_Providers.EventStore.EventstoreSnapshotStorageProvider
    {
        public override IEventStoreConnection GetEventStoreConnection()
        {
            return Util.EventstoreConnection.GetEventstoreConnection();
        }

        public override string GetStreamNamePrefix()
        {
            return Util.EventstoreConnection.GetSnapshotStreamPrefix();
        }
    }
}
