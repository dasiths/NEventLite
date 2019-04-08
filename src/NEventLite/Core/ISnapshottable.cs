using System;

namespace NEventLite.Core
{
    public interface ISnapshottable<TSnapshot> : ISnapshottable<TSnapshot, Guid, Guid> where TSnapshot : ISnapshot<Guid, Guid>
    {
    }

    public interface ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> where TSnapshot: ISnapshot<TAggregateKey, TSnapshotKey>
    {
        TSnapshot TakeSnapshot();
        void ApplySnapshot(TSnapshot snapshot);
    }
}