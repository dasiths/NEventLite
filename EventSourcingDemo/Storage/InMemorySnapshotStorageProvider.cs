using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Storage
{
    class InMemorySnapshotStorageProvider:ISnapshotStorageProvider{

        private readonly Dictionary<Guid,Snapshot.Snapshot> _items = new Dictionary<Guid,Snapshot.Snapshot>();

        public Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {
            if (_items.ContainsKey(aggregateId))
            {
                return _items[aggregateId];
            }
            else
            {
                return null;
            }
        }

        public void SaveSnapshot(Snapshot.Snapshot snapshot)
        {
            if (_items.ContainsKey(snapshot.AggregateId))
            {
               _items[snapshot.AggregateId] = snapshot;
            }
            else
            {
               _items.Add(snapshot.AggregateId,snapshot);
            }
        }
    }
}
