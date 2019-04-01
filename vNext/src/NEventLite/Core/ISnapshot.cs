namespace NEventLite.Core
{
    public interface ISnapshot<out TSnapshotKey, out TAggregateKey>
    {
        TSnapshotKey Id { get; }
        TAggregateKey AggregateId { get; }
        int Version { get; }
    }
}