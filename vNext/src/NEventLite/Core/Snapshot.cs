namespace NEventLite.Core
{
    public abstract class Snapshot<TSnapshotKey, TAggregateKey>
    {
        public TSnapshotKey Id { get; private set; }
        public TAggregateKey AggregateId { get; private set; }
        public int Version { get; private set; }

        protected Snapshot()
        {
        }

        protected Snapshot(TSnapshotKey id, TAggregateKey aggregateId, int version) : base()
        {
            Id = id;
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
