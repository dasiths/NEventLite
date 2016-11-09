using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Event_Bus;
using NEventLite.Exceptions;
using NEventLite.Extensions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        public IEventStorageProvider EventStorageProvider { get; }
        public ISnapshotStorageProvider SnapshotStorageProvider { get; }

        public IEventBus EventBus { get; }

        public Repository(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider, IEventBus eventBus)
        {
            EventStorageProvider = eventStorageProvider;
            SnapshotStorageProvider = snapshotStorageProvider;
            EventBus = eventBus;
        }

        public virtual T GetById(Guid id)
        {
            T item = null;

            var isSnapshottable = typeof(ISnapshottable).IsAssignableFrom(typeof(T));
            Snapshot.Snapshot snapshot = null;

            if ((isSnapshottable) && (SnapshotStorageProvider != null))
            {
                snapshot = SnapshotStorageProvider.GetSnapshot(typeof(T), id);
            }

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = EventStorageProvider.GetEvents(typeof(T), id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = EventStorageProvider.GetEvents(typeof(T), id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = new T();
                    item.LoadsFromHistory(events);
                }
            }

            return item;
        }

        public virtual void Save(T aggregate)
        {
            CommitChanges(aggregate);
        }

        private IEnumerable<IEvent> CommitChanges(AggregateRoot aggregate)
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

            var changesToCommit = aggregate.GetUncommittedChanges().ToList();

            EventStorageProvider.CommitChanges(aggregate.GetType(), aggregate);

            //If the Aggregate implements snaphottable
            var snapshottable = aggregate as ISnapshottable;

            if ((snapshottable != null) && (SnapshotStorageProvider != null))
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion >= SnapshotStorageProvider.SnapshotFrequency) &&
                        (
                            (changesToCommit.Count >= SnapshotStorageProvider.SnapshotFrequency) ||
                            (aggregate.CurrentVersion % SnapshotStorageProvider.SnapshotFrequency < changesToCommit.Count) ||
                            (aggregate.CurrentVersion % SnapshotStorageProvider.SnapshotFrequency == 0)
                        )
                    )
                {
                    SnapshotStorageProvider.SaveSnapshot(aggregate.GetType(), snapshottable.GetSnapshot());
                }
            }

            //Publish to event bus
            PublishToEventBus(changesToCommit);

            aggregate.MarkChangesAsCommitted();

            return changesToCommit;
        }

        private void PublishToEventBus(List<IEvent> changesToCommit)
        {
            EventBus.Publish(changesToCommit);
        }
    }
}
