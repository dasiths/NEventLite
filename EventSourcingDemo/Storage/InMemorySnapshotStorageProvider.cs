using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Storage
{
    class InMemorySnapshotStorageProvider:ISnapshotStorageProvider
    {

        private Dictionary<Guid,Snapshot.Snapshot> items = new Dictionary<Guid,Snapshot.Snapshot>();

        public Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {
            if (items.ContainsKey(aggregateId))
            {
                return items[aggregateId];
            }
            else
            {
                return null;
            }
        }

        public void SaveSnapshot(Snapshot.Snapshot snapshot)
        {
            if (items.ContainsKey(snapshot.AggregateId))
            {
               items[snapshot.AggregateId] = snapshot;
            }
            else
            {
                items.Add(snapshot.AggregateId,snapshot);
            }
        }
    }
}
