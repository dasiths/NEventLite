using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class EventOnlyRepository<TAggregate, TAggregateKey, TEventKey> : Repository<TAggregate, TAggregateKey, TEventKey, IMockSnapShot<TAggregateKey>, IMockAggregateKeyType>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        public EventOnlyRepository(IClock clock,
            IEventStorageProvider<TEventKey, TAggregate, TAggregateKey> eventStorageProvider,
            IEventPublisher<TEventKey, TAggregate, TAggregateKey> eventPublisher) : base(clock, eventStorageProvider, eventPublisher, null)
        {
        }
    }

    public interface IMockSnapShot<out TAggregateKey> : ISnapshot<IMockAggregateKeyType, TAggregateKey>
    {
    }

    public interface IMockAggregateKeyType
    {
    }
}
