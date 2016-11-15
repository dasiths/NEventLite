using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Event_Bus;
using NEventLite.Exceptions;
using NEventLite.Extensions;
using NEventLite.Logger;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository: IRepository
    {
        public IEventStorageProvider EventStorageProvider { get; }

        public ISnapshotStorageProvider SnapshotStorageProvider { get; }

        public IEventPublisher EventPublisher { get; }

        public Repository(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider, IEventPublisher eventPublisher)
        {
            EventStorageProvider = eventStorageProvider;
            SnapshotStorageProvider = snapshotStorageProvider;
            EventPublisher = eventPublisher;
        }

        public virtual T GetById<T>(Guid id) where T:AggregateRoot
        {
            T item = default(T);

            var isSnapshottable = typeof(ISnapshottable).IsAssignableFrom(typeof(T));
            Snapshot.Snapshot snapshot = null;

            if ((isSnapshottable) && (SnapshotStorageProvider != null))
            {
                snapshot = SnapshotStorageProvider.GetSnapshot(typeof(T), id);
            }

            if (snapshot != null)
            {
                item = CreateInstance<T>();
                ((ISnapshottable)item).ApplySnapshot(snapshot);
                var events = EventStorageProvider.GetEvents(typeof(T), id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = EventStorageProvider.GetEvents(typeof(T), id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = CreateInstance<T>();
                    item.LoadsFromHistory(events);
                }
            }

            return item;
        }

        public virtual void Save<T>(T aggregate) where T:AggregateRoot
        {
            if (aggregate.HasUncommittedChanges())
            {
                CommitChanges(aggregate);
            }
        }

        private void CommitChanges(AggregateRoot aggregate)
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
                    SnapshotStorageProvider.SaveSnapshot(aggregate.GetType(), snapshottable.TakeSnapshot());
                }
            }

            //Publish to event publisher asynchronously
            PublishToEventBus(changesToCommit.ToList());

            aggregate.MarkChangesAsCommitted();
        }

        private static T CreateInstance<T>() where T : AggregateRoot
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        private void PublishToEventBus(List<IEvent> changesToCommit)
        {
            EventPublisher.Publish(changesToCommit);
        }
    }
}
