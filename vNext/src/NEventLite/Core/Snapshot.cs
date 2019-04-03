﻿namespace NEventLite.Core
{
    public abstract class Snapshot<TAggregateKey, TSnapshotKey> : ISnapshot<TAggregateKey, TSnapshotKey>
    {
        public TSnapshotKey Id { get; private set; }
        public TAggregateKey AggregateId { get; private set; }
        public int Version { get; private set; }

        protected Snapshot(TSnapshotKey id, TAggregateKey aggregateId, int version)
        {
            Id = id;
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
