using System;
using System.Collections.Generic;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Exceptions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        public IEventStorageProvider EventStorageProvider { get; }
        public ISnapshotStorageProvider SnapshotStorageProvider { get; }

        private DateTime CommitStartTime;

        public Repository(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider)
        {
            EventStorageProvider = eventStorageProvider;
            SnapshotStorageProvider = snapshotStorageProvider;
        }

        public T GetById(Guid id)
        {

            T item = null;
            var snapshot = SnapshotStorageProvider.GetSnapshot(typeof(T), id);

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = EventStorageProvider.GetEvents(typeof(T),id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = EventStorageProvider.GetEvents(typeof(T),id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = new T();
                    item.LoadsFromHistory(events);
                }
            }

            return item;
        }

        public void Save(T aggregate)
        {
            HandlePreCommited(aggregate);
            HandlePostCommited(CommitToStorage(aggregate));
        }

        public void HandlePreCommited(T aggregate)
        {
            CommitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.");
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
