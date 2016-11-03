using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider:ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Guid aggregateId, int version);
    }
}
