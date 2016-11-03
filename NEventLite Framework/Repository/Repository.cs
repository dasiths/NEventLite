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
        private static readonly int snapshotFrequency = 5;

        public Repository(IEventStorageProvider eventStorageProvider, ISnapshotStorageProvider snapshotStorageProvider)
        {
            if (!eventStorageProvider.HasConcurrencyCheck)
                syncLockObject = new object();

            _EventStorageProvider = eventStorageProvider;
            _SnapshotStorageProvider = snapshotStorageProvider;
        }

        public void Save(AggregateRoot aggregate)
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                if (_EventStorageProvider.HasConcurrencyCheck)
                    this.DoSave(aggregate);
                else
                    lock (syncLockObject)
                        this.DoSave(aggregate);
            }
        }

        private void DoSave(AggregateRoot aggregate)
        {
            var item = new T();
            var expectedVersion = aggregate.LastCommittedVersion;

            if (expectedVersion != 0)
            {
                item = GetById(aggregate.Id);
                if ((item != null) && (item.CurrentVersion != expectedVersion))
                {
                    throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
                }
            }

            _EventStorageProvider.CommitChanges(aggregate);


            //If the Aggregate implements snaphottable
            var snapshottable = aggregate as ISnapshottable;

            if (snapshottable != null)
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion > (snapshotFrequency - 1)) &&
                    (aggregate.CurrentVersion - aggregate.LastCommittedVersion > snapshotFrequency) || (aggregate.CurrentVersion % snapshotFrequency == 0))
                {
                    _SnapshotStorageProvider.SaveSnapshot(snapshottable.GetSnapshot());
                }
            }


            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(Guid id)
        {

            T item = null;
            var snapshot = _SnapshotStorageProvider.GetSnapshot(id);

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = _EventStorageProvider.GetEvents(id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = _EventStorageProvider.GetEvents(id, 0, int.MaxValue);

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
