using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Storage
{
    class InMemorySnapshotStorageProvider:ISnapshotStorageProvider
    {
        public Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void SaveSnapshot(Snapshot.Snapshot snapshot)
        {
            throw new NotImplementedException();
        }
    }
}
