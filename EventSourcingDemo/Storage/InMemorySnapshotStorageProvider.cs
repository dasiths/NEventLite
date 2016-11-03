using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Storage;

namespace EventSourcingDemo.Storage
{
    class InMemorySnapshotStorageProvider:ISnapshotStorageProvider{

        private readonly Dictionary<Guid,NEventLite.Snapshot.Snapshot> _items = new Dictionary<Guid,NEventLite.Snapshot.Snapshot>();

        public NEventLite.Snapshot.Snapshot GetSnapshot(Guid aggregateId)
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

        public void SaveSnapshot(NEventLite.Snapshot.Snapshot snapshot)
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
