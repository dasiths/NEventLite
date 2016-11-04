using System;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Exceptions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStorageProvider _EventStorageProvider;
        private readonly ISnapshotStorageProvider _SnapshotStorageProvider;
        private static object syncLockObject;
        private static readonly int defaultSnapshotFrequency = 5;

        public int SnapshotFrequency { get; set; }

        public Repository(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider)
        {
            if (!eventStorageProvider.HasConcurrencyCheck)
                syncLockObject = new object();

            _EventStorageProvider = eventStorageProvider;
            _SnapshotStorageProvider = snapshotStorageProvider;
            SnapshotFrequency = defaultSnapshotFrequency;
        }

        public void Save(T aggregate)
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                if (_EventStorageProvider.HasConcurrencyCheck)
                    this.SaveAggregate(aggregate);
                else
                    lock (syncLockObject)
                        this.SaveAggregate(aggregate);
            }
        }

        private void SaveAggregate(AggregateRoot aggregate)
        {
            var expectedVersion = aggregate.LastCommittedVersion;

            T item = GetById(aggregate.Id);

            if ((item != null) && (expectedVersion == (int)AggregateRoot.StreamState.NoStream))
            {
                throw new AggregateCreationException($"Aggregate {item.Id} can't be created as it already exists with version {item.CurrentVersion}");
            }
            else if ((item != null) && (item.CurrentVersion != expectedVersion))
            {
                throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
            }

            _EventStorageProvider.CommitChanges(aggregate);


            //If the Aggregate implements snaphottable
            var snapshottable = aggregate as ISnapshottable;

            if (snapshottable != null)
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion >= SnapshotFrequency) &&
                    (aggregate.CurrentVersion - aggregate.LastCommittedVersion > SnapshotFrequency) || (aggregate.CurrentVersion % SnapshotFrequency == 0))
                {
                    _SnapshotStorageProvider.SaveSnapshot<T>(snapshottable.GetSnapshot());
                }
            }


            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(Guid id)
        {

            T item = null;
            var snapshot = _SnapshotStorageProvider.GetSnapshot<T>(id);

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = _EventStorageProvider.GetEvents<T>(id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = _EventStorageProvider.GetEvents<T>(id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = new T();
                    item.LoadsFromHistory(events);
                }
            }

            return item;
        }
    }
}
