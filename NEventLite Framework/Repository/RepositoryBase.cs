using System;
using System.Collections.Generic;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Exceptions;
using NEventLite.Extensions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : AggregateRoot, new()
    {
        public List<Action<Guid>> PreLoadActions { get; private set; }
        public List<Action<T>> PostLoadActions { get; private set; }
        public List<Action<T>> PreCommitActions { get; private set; }
        public List<Action<T, IEnumerable<IEvent>>> PostCommitActions { get; private set; }

        public IEventStorageProvider EventStorageProvider { get; }
        public ISnapshotStorageProvider SnapshotStorageProvider { get; }

        public RepositoryBase(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider)
        {
            EventStorageProvider = eventStorageProvider;
            SnapshotStorageProvider = snapshotStorageProvider;
            PreLoadActions = new List<Action<Guid>>();
            PostLoadActions = new List<Action<T>>();
            PreCommitActions = new List<Action<T>>();
            PostCommitActions = new List<Action<T, IEnumerable<IEvent>>>();
        }

        public T GetById(Guid id)
        {
            id.ApplyActions(PreLoadActions);

            T item = null;
            var snapshot = SnapshotStorageProvider.GetSnapshot(typeof(T), id);

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

            item?.ApplyActions(PostLoadActions);

            return item;
        }

        public void Save(T aggregate)
        {
            aggregate.ApplyActions(PreCommitActions);
            aggregate.ApplyActionsWithArgument(PostCommitActions, CommitToStorage(aggregate));
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
