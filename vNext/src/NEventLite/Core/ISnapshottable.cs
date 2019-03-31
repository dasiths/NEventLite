namespace NEventLite.Core
{
    public interface ISnapshottable<TSnapshotKey, TAggregateKey>
    {
        Snapshot<TSnapshotKey, TAggregateKey> TakeSnapshot();
        void ApplySnapshot(Snapshot<TSnapshotKey, TAggregateKey> snapshot);
    }
}