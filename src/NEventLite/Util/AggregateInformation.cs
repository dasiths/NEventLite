using System;

namespace NEventLite.Util
{
    public class AggregateInformation
    {
        public readonly bool IsValidResult;

        public Type Aggregate { get; set; }
        public Type AggregateKey { get; set; }
        public Type EventKey { get; set; }
        public Type Snapshot { get; set; }
        public Type SnapshotKey { get; set; }

        private AggregateInformation(Type aggregate, Type aggregateKey, Type eventKey, Type snapshot, Type snapshotKey)
        {
            IsValidResult = true;
            Aggregate = aggregate;
            AggregateKey = aggregateKey;
            EventKey = eventKey;
            Snapshot = snapshot;
            SnapshotKey = snapshotKey;
        }

        private AggregateInformation()
        {
            IsValidResult = false;
        }

        public static AggregateInformation ValidResult(Type aggregate, Type aggregateKey, Type eventKey, Type snapshot, Type snapshotKey) 
            => new AggregateInformation(aggregate, aggregateKey, eventKey, snapshot, snapshotKey);

        public static AggregateInformation InvalidResult => new AggregateInformation();
    }
}