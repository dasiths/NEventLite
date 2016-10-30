using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;

namespace EventSourcingDemo.Storage
{
    interface IEventStorageProvider
    {
        IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end);
        void CommitChanges(AggregateRoot aggregate);
    }
}
