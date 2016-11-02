using EventSourcingDemo.Domain;
using EventSourcingDemo.Exceptions;
using EventSourcingDemo.Snapshot;
using EventSourcingDemo.Storage;
using System;
using System.Linq;

namespace EventSourcingDemo.Repository
{
    class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStorageProvider _EventStorageProvider;
        private readonly ISnapshotStorageProvider _SnapshotStorageProvider;
        private static object syncLockObject;

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
                if (item.CurrentVersion != expectedVersion)
                {
                    throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
                }
            }

            _EventStorageProvider.CommitChanges(aggregate);

            //Every 3 events we save a snapshot
            if ((aggregate.CurrentVersion > 2) &&
                (aggregate.CurrentVersion - aggregate.LastCommittedVersion > 3) || (aggregate.CurrentVersion % 3 == 0))
            {
                _SnapshotStorageProvider.SaveSnapshot(((ISnapshottable)aggregate).GetSnapshot());
            }

            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(Guid id)
        {

            var item = new T();
            var snapshot = _SnapshotStorageProvider.GetSnapshot(id);

            if (snapshot != null)
            {
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = _EventStorageProvider.GetEvents(id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
            }
            else
            {
                var events = _EventStorageProvider.GetEvents(id, 0, int.MaxValue);
                item.LoadsFromHistory(events);
            }

            return item;
        }
    }
}
