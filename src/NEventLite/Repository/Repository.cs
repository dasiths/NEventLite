using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Exceptions;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<TAggregate, TSnapshot> :
        Repository<TAggregate, TSnapshot, Guid, Guid, Guid>
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
            TAggregate item;
            var snapshot = default(TSnapshot);

            var isSnapshottable =
                typeof(ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey>).IsAssignableFrom(typeof(TAggregate));
            
            if (isSnapshottable)
            {
                snapshot = await GetLatestSnapshotAsync(id);
            }
            
            if (snapshot != null)
            {
                item = CreateNewInstance();

                if (!(item is ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> snapshottableItem))
                {
                    throw new Exception($"{nameof(snapshottableItem)} is not of ISnapshottable<{typeof(TSnapshot).Name}, {typeof(TAggregateKey).Name}, {typeof(TSnapshotKey).Name}>");
                }

                item.HydrateFromSnapshot(snapshot);
                snapshottableItem.ApplySnapshot(snapshot);

                var events = await _eventStorageProvider.GetEventsAsync<TAggregate, TAggregateKey>(id, snapshot.Version + 1, long.MaxValue);
                await item.LoadsFromHistoryAsync(events);
            }
            else
            {
                var events = (await _eventStorageProvider.GetEventsAsync<TAggregate, TAggregateKey>(id, 0, long.MaxValue)).ToList();

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
                if (expectedVersion == (long)StreamState.NoStream)
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
                .Cast<IEvent<TAggregate, TAggregateKey, TEventKey>>()
                .ToList();

            //perform pre commit actions
            DoPreCommitTasks(changesToCommit);

            //CommitAsync events to storage provider
            await _eventStorageProvider.SaveAsync<TAggregate, TAggregateKey>(aggregate.Id, changesToCommit);

            //Publish to event publisher asynchronously
            await PublishEventsAsync(changesToCommit);

            //If the Aggregate implements snapshottable
            await SaveSnapshotAsync(aggregate, changesToCommit);

            //Finally mark them committed
            aggregate.MarkChangesAsCommitted();
        }

        private async Task<TSnapshot> GetLatestSnapshotAsync(TAggregateKey aggregateId)
        {
            var snapshot = await _snapshotStorageProvider.GetSnapshotAsync<TSnapshot, TAggregateKey>(aggregateId);
            return snapshot;
        }

        private async Task SaveSnapshotAsync(TAggregate aggregate, ICollection changesToCommit)
        {
            if (aggregate is ISnapshottable<TSnapshot, TAggregateKey, TSnapshotKey> snapshottable)
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
        }

        private void DoPreCommitTasks(IEnumerable<IEvent<TAggregate, TAggregateKey, TEventKey>> events)
        {
            foreach (var e in events)
            {
                e.EventCommittedTimestamp = _clock.Now();
            }
        }

        private async Task PublishEventsAsync(IEnumerable<IEvent<TAggregate, TAggregateKey, TEventKey>> events)
        {
            if (_eventPublisher != null)
            {
                foreach (var e in events)
                {
                    await _eventPublisher.PublishAsync(e);
                }
            }
        }

        private static TAggregate CreateNewInstance()
        {
            return new TAggregate();
        }
    }
}
