namespace NEventLite.Snapshot
{
    public interface ISnapshottable
    {
        Snapshot GetSnapshot();
        void SetSnapshot(Snapshot snapshot);
    }
}
