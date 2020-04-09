using System;

namespace NEventLite.Core
{
    public interface ISnapshot<out TAggregateKey> : ISnapshot<TAggregateKey, Guid>
    {
    }

    public interface ISnapshot<out TAggregateKey, out TSnapshotKey>: ISnapshot
    {
        TSnapshotKey Id { get; }
        TAggregateKey AggregateId { get; }
    }

    public interface ISnapshot
    {
        int Version { get; }
    }
}