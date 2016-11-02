using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace EventSourcingDemo.Util
{
    public static class EventstoreConnection
    {
        public static IEventStoreConnection GetEventstoreConnection()
        {
            //Connection to the local eventstore on default port 1113
            return EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
        }
    }
}
