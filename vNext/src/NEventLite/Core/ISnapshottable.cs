namespace NEventLite.Core
{
    public interface ISnapshottable<TSnapshotKey, TAggregateKey, TSnapshot> where TSnapshot: ISnapshot<TSnapshotKey, TAggregateKey>
    {
        TSnapshot TakeSnapshot();
        void ApplySnapshot(TSnapshot snapshot);
    }
}