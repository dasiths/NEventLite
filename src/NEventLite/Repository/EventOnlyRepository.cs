using System;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class EventOnlyRepository<TAggregate, TAggregateKey, TEventKey> : Repository<TAggregate, IMockSnapShot<TAggregateKey>, TAggregateKey, TEventKey, IMockSnapshotKeyType>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        public EventOnlyRepository(IClock clock,
            IEventStorageProvider<TEventKey> eventStorageProvider,
            IEventPublisher eventPublisher) : base(clock, eventStorageProvider, eventPublisher, new MockSnapshotStorageProvider())
        {
        }
    }

    internal sealed class MockSnapshotStorageProvider : ISnapshotStorageProvider<IMockSnapshotKeyType>
    {
        public int SnapshotFrequency { get; } = int.MaxValue;
        public async Task<TSnapshot> GetSnapshotAsync<TSnapshot, TAggregateKey>(TAggregateKey aggregateId) where TSnapshot : ISnapshot<TAggregateKey, IMockSnapshotKeyType>
        {
            return default(TSnapshot);
        }

        public async Task SaveSnapshotAsync<TSnapshot, TAggregateKey>(TSnapshot snapshot) where TSnapshot : ISnapshot<TAggregateKey, IMockSnapshotKeyType>
        {
        }
    }

    public interface IMockSnapShot<out TAggregateKey> : ISnapshot<TAggregateKey, IMockSnapshotKeyType>
    {
    }

    public interface IMockSnapshotKeyType
    {
    }
}
