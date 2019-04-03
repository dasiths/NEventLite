using System;

namespace NEventLite.Core
{
    public interface ISnapshot<out TAggregate> : ISnapshot<TAggregate, Guid>
    {
    }

    public interface ISnapshot<out TAggregateKey, out TSnapshotKey>
    {
        TSnapshotKey Id { get; }
        TAggregateKey AggregateId { get; }
        int Version { get; }
    }
}