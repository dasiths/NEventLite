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
    public class Repository<TAggregate, TAggregateKey, TEventKey, TSnapshotKey> :
        IRepository<TAggregate, TAggregateKey, TEventKey, TSnapshotKey> where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        private readonly IEventStorageProvider<TEventKey, TAggregateKey> _eventStorageProvider;
        private readonly ISnapshotStorageProvider<TSnapshotKey, TAggregateKey> _snapshotStorageProvider;
        private readonly IEventPublisher<TEventKey, TAggregateKey> _eventPublisher;
        private readonly IClock _clock;

        public Repository(IEventStorageProvider<TEventKey, TAggregateKey> eventStorageProvider,
            IClock clock) : this(eventStorageProvider, null, null, clock)
        {
        }

        public Repository(IEventStorageProvider<TEventKey, TAggregateKey> eventStorageProvider,
            IEventPublisher<TEventKey, TAggregateKey> eventPublisher,
            IClock clock) : this(eventStorageProvider, null, eventPublisher, clock)
        {
        }

        public Repository(IEventStorageProvider<TEventKey, TAggregateKey> eventStorageProvider,
            ISnapshotStorageProvider<TSnapshotKey, TAggregateKey> snapshotStorageProvider,
            IClock clock) : this(eventStorageProvider, snapshotStorageProvider, null, clock)
        {
        }

        public Repository(IEventStorageProvider<TEventKey, TAggregateKey> eventStorageProvider,
            ISnapshotStorageProvider<TSnapshotKey, TAggregateKey> snapshotStorageProvider,
            IEventPublisher<TEventKey, TAggregateKey> eventPublisher,
            IClock clock)
        {
            _eventStorageProvider = eventStorageProvider;
            _snapshotStorageProvider = snapshotStorageProvider;
            _eventPublisher = eventPublisher;
            _clock = clock;
        }

        public async Task<TAggregate> GetByIdAsync(TAggregateKey id)
        {
            var item = default(TAggregate);
            var isSnapshottable = ((item as ISnapshottable<TSnapshotKey, TAggregateKey>) == null);

            Snapshot<TSnapshotKey, TAggregateKey> snapshot = null;

            if ((isSnapshottable) && (_snapshotStorageProvider != null))
            {
                snapshot = await _snapshotStorageProvider.GetSnapshotAsync(typeof(TAggregate), id);
            }

            if (snapshot != null)
            {
                item = ReflectionHelper.CreateInstance<TAggregate, TAggregateKey, TEventKey>();
                (item as ISnapshottable<TSnapshotKey, TAggregateKey>).ApplySnapshot(snapshot);

                var events = await _eventStorageProvider.GetEventsAsync(typeof(TAggregate), id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistoryAsync(events);
            }
            else
            {
                var events = (await _eventStorageProvider.GetEventsAsync(typeof(TAggregate), id, 0, int.MaxValue)).ToList();

                if (events.Any())
                {
                    item = new TAggregate();
                    item.LoadsFromHistoryAsync(events);
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

            var item = await _eventStorageProvider.GetLastEventAsync(aggregate.GetType(), aggregate.Id);

            if ((item != null) && (expectedVersion == (int)StreamState.NoStream))
            {
                throw new AggregateCreationException($"Aggregate {item.CorrelationId} can't be created as it already exists with version {item.TargetVersion + 1}");
            }
            else if ((item != null) && ((item.TargetVersion + 1) != expectedVersion))
            {
                throw new ConcurrencyException($"Aggregate {item.CorrelationId} has been modified externally and has an updated state. Can't commit changes.");
            }

            var changesToCommit = aggregate.GetUncommittedChanges().ToList();

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
            var snapshottable = aggregate as ISnapshottable<TSnapshotKey, TAggregateKey>;

            if ((snapshottable != null) && (_snapshotStorageProvider != null))
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
                    await _snapshotStorageProvider.SaveSnapshotAsync(aggregate.GetType(), snapshottable.TakeSnapshot());
                }
            }

            aggregate.MarkChangesAsCommitted();
        }

        private void DoPreCommitTasks(IEvent<TEventKey, TAggregateKey> e)
        {
            e.EventCommittedTimestamp = _clock.Now();
        }
    }
}
