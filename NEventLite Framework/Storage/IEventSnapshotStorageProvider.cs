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
        Snapshot.Snapshot GetSnapshot<T>(Guid aggregateId, int version) where T : AggregateRoot;
    }
}
