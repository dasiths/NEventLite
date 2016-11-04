using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;

namespace NEventLite.Storage
{
    public interface IEventSnapshotStorageProvider:ISnapshotStorageProvider
    {
        Snapshot.Snapshot GetSnapshot(Type aggregateType, Guid aggregateId, int version);
    }
}
