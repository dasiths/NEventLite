using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Exceptions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Session
{
    public class Session:ISession
    {
        public IEventStorageProvider EventStorageProvider { get; }
        public ISnapshotStorageProvider SnapshotStorageProvider { get; }

        private readonly Dictionary<Guid, AggregateRoot> _trackedItems;
        private DateTime CommitStartTime;

        public Session(IEventStorageProvider eventProvider, ISnapshotStorageProvider snapshotProvider)
        {
            EventStorageProvider = eventProvider;
            SnapshotStorageProvider = snapshotProvider;
            _trackedItems = new Dictionary<Guid, AggregateRoot>();
        }

        public void Add(AggregateRoot aggregate)
        {

            if (IsTracked(aggregate.Id))
            {
                throw new ConcurrencyException($"Aggregate {aggregate.Id} is already being tracked.");
            }
            else
            {
                _trackedItems.Add(aggregate.Id, aggregate);
            }

        }

        public bool IsTracked(Guid id)
        {
            return _trackedItems.ContainsKey(id);
        }

        public void HandlePreCommited()
        {
            CommitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {_trackedItems.Sum(o => o.Value.GetUncommittedChanges().Count())} events to storage.");
        }

        public void CommitChanges()
        {

            HandlePreCommited();

            List<IEvent> AllChanges = new List<IEvent>();

            foreach (var item in _trackedItems.Values)
            {
                if (item.GetUncommittedChanges().Any())
                {
                    AllChanges.AddRange(CommitToStorage(item));
                }
            }

            HandlePostCommited(AllChanges);
        }

        public void HandlePostCommited(IEnumerable<IEvent> events)
        {
            //Todo: Publish to EventBus
            Console.WriteLine($"Committed {events.Count()} events to storage in {DateTime.Now.Subtract(CommitStartTime).TotalMilliseconds} ms.");
        }

        private IEnumerable<IEvent> CommitToStorage(AggregateRoot aggregate)
        {
            var expectedVersion = aggregate.LastCommittedVersion;

            IEvent item = EventStorageProvider.GetLastEvent(aggregate.GetType(), aggregate.Id);

            if ((item != null) && (expectedVersion == (int)AggregateRoot.StreamState.NoStream))
            {
                throw new AggregateCreationException($"Aggregate {item.Id} can't be created as it already exists with version {item.TargetVersion + 1}");
            }
            else if ((item != null) && ((item.TargetVersion + 1) != expectedVersion))
            {
                throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
            }

            var ChangesToCommit = aggregate.GetUncommittedChanges();
            EventStorageProvider.CommitChanges(aggregate.GetType(), aggregate);

            //If the Aggregate implements snaphottable
            var snapshottable = aggregate as ISnapshottable;

            if (snapshottable != null)
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion >= SnapshotStorageProvider.SnapshotFrequency) &&
                    (aggregate.CurrentVersion - aggregate.LastCommittedVersion > SnapshotStorageProvider.SnapshotFrequency) || (aggregate.CurrentVersion % SnapshotStorageProvider.SnapshotFrequency == 0))
                {
                    SnapshotStorageProvider.SaveSnapshot(aggregate.GetType(), snapshottable.GetSnapshot());
                }
            }

            aggregate.MarkChangesAsCommitted();

            return ChangesToCommit;
        }


    }
}
