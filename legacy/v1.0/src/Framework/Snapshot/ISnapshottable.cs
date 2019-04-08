namespace NEventLite.Snapshot
{
    public interface ISnapshottable
    {
        Snapshot TakeSnapshot();
        void ApplySnapshot(Snapshot snapshot);
    }
}
