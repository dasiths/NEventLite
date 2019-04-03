namespace NEventLite.Core
{
    public interface ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> where TSnapshot: ISnapshot<TAggregateKey, TSnapshotKey>
    {
        TSnapshot TakeSnapshot();
        void ApplySnapshot(TSnapshot snapshot);
    }
}