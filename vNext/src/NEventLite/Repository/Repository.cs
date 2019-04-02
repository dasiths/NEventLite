using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Exceptions;
using NEventLite.Storage;
using NEventLite.Util;

namespace NEventLite.Repository
{
    public class Repository<TAggregate, TAggregateKey, TEventKey, TSnapshotKey, TSnapshot> :
        IRepository<TAggregate, TAggregateKey, TEventKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
        where TSnapshot : ISnapshot<TSnapshotKey, TAggregateKey>
    {
        private readonly IEventStorageProvider<TEventKey, TAggregate, TAggregateKey> _eventStorageProvider;
        private readonly ISnapshotStorageProvider<TSnapshot, TSnapshotKey, TAggregateKey> _snapshotStorageProvider;
        private readonly IEventPublisher<TEventKey, TAggregate, TAggregateKey> _eventPublisher;
        private readonly IClock _clock;

        public Repository(IClock clock,
            IEventStorageProvider<TEventKey, TAggregate, TAggregateKey> eventStorageProvider,
            IEventPublisher<TEventKey, TAggregate, TAggregateKey> eventPublisher,
            ISnapshotStorageProvider<TSnapshot, TSnapshotKey, TAggregateKey> snapshotStorageProvider)
        {
            _eventStorageProvider = eventStorageProvider;
            _snapshotStorageProvider = snapshotStorageProvider;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task<TAggregate> GetByIdAsync(TAggregateKey id)
        {
            var item = default(TAggregate);
            var isSnapshottable =
                typeof(ISnapshottable<TSnapshotKey, TAggregateKey, TSnapshot>).IsAssignableFrom(typeof(TAggregate));

            var snapshot = default(TSnapshot);

            if ((isSnapshottable) && (_snapshotStorageProvider != null))
            {
                snapshot = await _snapshotStorageProvider.GetSnapshotAsync(id);
            }

            if (snapshot != null)
            {
                item = CreateNewInstance();
                var snapshottableItem = (item as ISnapshottable<TSnapshotKey, TAggregateKey, TSnapshot>);

                if (snapshottableItem == null)
                {
                    throw new NullReferenceException(nameof(snapshottableItem));
                }

                item.HydrateFromSnapshot(snapshot);
                snapshottableItem.ApplySnapshot(snapshot);

                var events = await _eventStorageProvider.GetEventsAsync(id, snapshot.Version + 1, int.MaxValue);
                await item.LoadsFromHistoryAsync(events);
            }
            else
            {
                var events = (await _eventStorageProvider.GetEventsAsync(id, 0, int.MaxValue)).ToList();

                if (events.Any())
                {
                    item = CreateNewInstance();
                    await item.LoadsFromHistoryAsync(events);
                }
            }

            return item;
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            if (aggregate.HasUncommittedChanges())
            {
                await CommitChanges(aggregate);
            }
        }

        private async Task CommitChanges(AggregateRoot<TAggregateKey, TEventKey> aggregate)
        {
            var expectedVersion = aggregate.LastCommittedVersion;

            var item = await _eventStorageProvider.GetLastEventAsync(aggregate.Id);

            if ((item != null) && (expectedVersion == (int)StreamState.NoStream))
            {
                throw new AggregateCreationException($"Aggregate {item.CorrelationId} can't be created as it already exists with version {item.TargetVersion + 1}");
            }
            else if ((item != null) && ((item.TargetVersion + 1) != expectedVersion))
            {
                throw new ConcurrencyException($"Aggregate {item.CorrelationId} has been modified externally and has an updated state. Can't commit changes.");
            }

            var changesToCommit = aggregate
                .GetUncommittedChanges()
                .Select(e => (IEvent<TEventKey, TAggregate, TAggregateKey>)e)
                .ToList();

            //perform pre commit actions
            foreach (var e in changesToCommit)
            {
                DoPreCommitTasks(e);
            }

            //CommitAsync events to storage provider
            await _eventStorageProvider.SaveAsync(aggregate);

            //Publish to event publisher asynchronously
            foreach (var e in changesToCommit)
            {
                if (_eventPublisher != null)
                {
                    await _eventPublisher.PublishAsync(e);
                }
            }

            //If the Aggregate implements snapshottable

            if ((aggregate is ISnapshottable<TSnapshotKey, TAggregateKey, TSnapshot> snapshottable) && (_snapshotStorageProvider != null))
            {
                //Every N events we save a snapshot
                if ((aggregate.CurrentVersion >= _snapshotStorageProvider.SnapshotFrequency) &&
                        (
                            (changesToCommit.Count >= _snapshotStorageProvider.SnapshotFrequency) ||
                            (aggregate.CurrentVersion % _snapshotStorageProvider.SnapshotFrequency < changesToCommit.Count) ||
                            (aggregate.CurrentVersion % _snapshotStorageProvider.SnapshotFrequency == 0)
                        )
                    )
                {
                    var snapshot = snapshottable.TakeSnapshot();
                    await _snapshotStorageProvider.SaveSnapshotAsync(snapshot);
                }
            }

            aggregate.MarkChangesAsCommitted();
        }

        private TAggregate CreateNewInstance()
        {
            return new TAggregate();
        }

        private void DoPreCommitTasks(IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey> e)
        {
            e.EventCommittedTimestamp = _clock.Now();
        }
    }
}
