using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using EventSourcingDemo.Snapshot;

namespace EventSourcingDemo.Storage
{
    public interface IEventStorageProvider
    {
        ISnapshotStorageProvider snapshotStorage { get; set; }
        IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end);
        void CommitChanges(AggregateRoot aggregate);
    }
}


