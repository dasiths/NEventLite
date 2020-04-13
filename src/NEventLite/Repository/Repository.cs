using System;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Exceptions;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<TAggregate, TSnapshot> :
        Repository<TAggregate, TSnapshot, Guid, Guid, Guid>,
        IRepository<TAggregate>
        where TAggregate : AggregateRoot<Guid, Guid>, new()
        where TSnapshot : ISnapshot<Guid, Guid>
    {
        public Repository(IClock clock,
            IEventStorageProvider<Guid> eventStorageProvider,
            IEventPublisher eventPublisher,
            ISnapshotStorageProvider<Guid> snapshotStorageProvider) :
            base(clock, eventStorageProvider, eventPublisher, snapshotStorageProvider)
        {
        }

        public Repository(IClock clock,
            IEventStorageProvider eventStorageProvider,
            IEventPublisher eventPublisher,
            ISnapshotStorageProvider snapshotStorageProvider) :
            base(clock, eventStorageProvider, eventPublisher, snapshotStorageProvider)
        {
        }
    }

    public class Repository<TAggregate, TSnapshot, TAggregateKey, TEventKey, TSnapshotKey> :
        IRepository<TAggregate, TAggregateKey, TEventKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
        where TSnapshot : ISnapshot<TAggregateKey, TSnapshotKey>
    {
        private readonly IEventStorageProvider<TEventKey> _eventStorageProvider;
        private readonly ISnapshotStorageProvider<TSnapshotKey> _snapshotStorageProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly IClock _clock;

        public Repository(IClock clock,
            IEventStorageProvider<TEventKey> eventStorageProvider,
            IEventPublisher eventPublisher,
            ISnapshotStorageProvider<TSnapshotKey> snapshotStorageProvider)
        {
            _eventStorageProvider = eventStorageProvider;
            _snapshotStorageProvider = snapshotStorageProvider;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task<TAggregate> GetByIdAsync(TAggregateKey id)
        {
            var isSnapshottable =
                typeof(ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey>).IsAssignableFrom(typeof(TAggregate));

            var snapshot = default(TSnapshot);

            if ((isSnapshottable) && (_snapshotStorageProvider != null))
            {
                snapshot = await _snapshotStorageProvider.GetSnapshotAsync<TSnapshot, TAggregateKey>(id);
            }

            TAggregate item;

            if (snapshot != null)
            {
                item = CreateNewInstance();

                if (!(item is ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> snapshottableItem))
                {
                    throw new NullReferenceException(nameof(snapshottableItem));
                }

                item.HydrateFromSnapshot(snapshot);
                snapshottableItem.ApplySnapshot(snapshot);

                var events = await _eventStorageProvider.GetEventsAsync<TAggregate, TAggregateKey>(id, snapshot.Version + 1, int.MaxValue);
                await item.LoadsFromHistoryAsync(events);
            }
            else
            {
                var events = (await _eventStorageProvider.GetEventsAsync<TAggregate, TAggregateKey>(id, 0, int.MaxValue)).ToList();

                if (events.Any())
                {
                    item = CreateNewInstance();
                    await item.LoadsFromHistoryAsync(events);
                }
                else
                {
                    throw new AggregateNotFoundException($"No events for the aggregate with id={id} were found.");
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

        private async Task CommitChanges(TAggregate aggregate)
        {
            var expectedVersion = aggregate.LastCommittedVersion;

            var item = await _eventStorageProvider.GetLastEventAsync<TAggregate, TAggregateKey>(aggregate.Id);

            if ((item != null))
            {
                if (expectedVersion == (int)StreamState.NoStream)
                {
                    throw new AggregateCreationException($"Aggregate {item.CorrelationId} can't be created as it already exists with version {item.TargetVersion + 1}");
                }

                if ((item.TargetVersion + 1) != expectedVersion)
                {
                    throw new ConcurrencyException($"Aggregate {item.CorrelationId} has been modified externally and has an updated state. Can't commit changes.");
                }
            }

            var changesToCommit = aggregate
                .GetUncommittedChanges()
                .Select(e => (IEvent<TAggregate, TAggregateKey, TEventKey>)e)
                .ToList();

            //perform pre commit actions
            foreach (var e in changesToCommit)
            {
                DoPreCommitTasks(e);
            }

            //CommitAsync events to storage provider
            await _eventStorageProvider.SaveAsync<TAggregate, TAggregateKey>(aggregate);

            //Publish to event publisher asynchronously
            foreach (var e in changesToCommit)
            {
                if (_eventPublisher != null)
                {
                    await _eventPublisher.PublishAsync(e);
                }
            }

            //If the Aggregate implements snapshottable

            if ((_snapshotStorageProvider != null) && (aggregate is ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> snapshottable))
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
                    await _snapshotStorageProvider.SaveSnapshotAsync<TSnapshot, TAggregateKey>(snapshot);
                }
            }

            aggregate.MarkChangesAsCommitted();
        }

        private void DoPreCommitTasks(IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey> e)
        {
            e.EventCommittedTimestamp = _clock.Now();
        }

        private static TAggregate CreateNewInstance()
        {
            return new TAggregate();
        }
    }
}
