using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;

namespace EventSourcingDemo.Storage
{
    public interface ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Guid aggregateId);
        void SaveSnapshot(Snapshot.Snapshot snapshot);
    }
}
