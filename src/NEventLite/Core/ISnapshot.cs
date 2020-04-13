using System;

namespace NEventLite.Core
{
    // ISnapshot<,> does not need many generic definitions because they are implemented in the abstract Snapshot class.
    // Unlike IRepository<,> we don't register ISnapshot<,,> in the DI hence there is no need for convenience generic variants.

    public interface ISnapshot<out TAggregateKey, out TSnapshotKey>: ISnapshot
    {
        TSnapshotKey Id { get; }
        TAggregateKey AggregateId { get; }
    }

    public interface ISnapshot
    {
        int SchemaVersion { get; set; }
        long Version { get; }
    }
}