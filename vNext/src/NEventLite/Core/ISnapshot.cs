namespace NEventLite.Core
{
    public interface ISnapshot<TSnapshotKey, TAggregateKey>
    {
        TSnapshotKey Id { get; }
        TAggregateKey AggregateId { get; }
        int Version { get; }
    }
}