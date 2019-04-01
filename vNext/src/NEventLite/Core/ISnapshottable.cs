namespace NEventLite.Core
{
    public interface ISnapshottable<TSnapshotKey, TAggregateKey>
    {
        ISnapshot<TSnapshotKey, TAggregateKey> TakeSnapshot();
        void ApplySnapshot(ISnapshot<TSnapshotKey, TAggregateKey> snapshot);
    }
}