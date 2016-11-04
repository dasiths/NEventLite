using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Exceptions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class ChangeTrackingContext
    {
        public readonly IEventStorageProvider EventStorageProvider;
        public readonly ISnapshotStorageProvider SnapshotStorageProvider;

        public int SnapshotFrequency { get; set; }
        private static readonly int defaultSnapshotFrequency = 5;

        private readonly List<AggregateRoot> TrackedItems;

        public ChangeTrackingContext(IEventStorageProvider eventProvider, ISnapshotStorageProvider snapshotProvider)
        {
            EventStorageProvider = eventProvider;
            SnapshotStorageProvider = snapshotProvider;
            TrackedItems = new List<AggregateRoot>();
            SnapshotFrequency = defaultSnapshotFrequency;
        }

        public void AddTrackedAggregate(AggregateRoot aggregate)
        {
            var item = TrackedItems.SingleOrDefault(o => o.Id == aggregate.Id);

            if (item != null)
            {
                item.SetTracker(null);
                TrackedItems.Remove(item);
            }

            TrackedItems.Add(aggregate);
        }

        public void CommitChanges()
        {
            foreach (var item in TrackedItems)
            {
                if (item.GetUncommittedChanges().Any())
                {
                    SaveAggregateAndPublish(item);
                }
                
            }
        }

        private void SaveAggregateAndPublish(AggregateRoot aggregate)
        {
            var expectedVersion = aggregate.LastCommittedVersion;

            Events.Event item = EventStorageProvider.GetLastEvent(aggregate.GetType(), aggregate.Id);

            if ((item != null) && (expectedVersion == (int)AggregateRoot.StreamState.NoStream))
            {
                throw new AggregateCreationException($"Aggregate {item.Id} can't be created as it already exists with version {item.TargetVersion + 1}");
            }
            else if ((item != null) && ((item.TargetVersion + 1) != expectedVersion))
            {
                throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
            }

            EventStorageProvider.CommitChanges(aggregate.GetType(), aggregate);

            //If the Aggregate implements snaphottable
            var snapshottable = aggregate as ISnapshottable;

            if (snapshottable != null)
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion >= SnapshotFrequency) &&
                    (aggregate.CurrentVersion - aggregate.LastCommittedVersion > SnapshotFrequency) || (aggregate.CurrentVersion % SnapshotFrequency == 0))
                {
                    SnapshotStorageProvider.SaveSnapshot(aggregate.GetType(), snapshottable.GetSnapshot());
                }
            }

            aggregate.MarkChangesAsCommitted();
        }
    }
}
