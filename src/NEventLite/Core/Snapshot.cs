using System;

namespace NEventLite.Core
{
    public abstract class Snapshot : Snapshot<Guid, Guid>
    {
        protected Snapshot(Guid id, Guid aggregateId, long version) : base(id, aggregateId, version)
        {
        }
    }

    public abstract class Snapshot<TAggregateKey, TSnapshotKey> : ISnapshot<TAggregateKey, TSnapshotKey>
    {
        public TSnapshotKey Id { get; private set; }
        public TAggregateKey AggregateId { get; private set; }
        public int SchemaVersion { get; set; }
        public long Version { get; private set; }

        protected Snapshot(TSnapshotKey id, TAggregateKey aggregateId, long version)
        {
            Id = id;
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
