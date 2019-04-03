using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class EventOnlyRepository<TAggregate, TAggregateKey, TEventKey> : Repository<TAggregate, IMockSnapShot<TAggregateKey>, TAggregateKey, TEventKey, IMockAggregateKeyType>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        public EventOnlyRepository(IClock clock,
            IEventStorageProvider<TAggregate, TAggregateKey, TEventKey> eventStorageProvider,
            IEventPublisher<TAggregate, TAggregateKey, TEventKey> eventPublisher) : base(clock, eventStorageProvider, eventPublisher, null)
        {
        }
    }

    public interface IMockSnapShot<out TAggregateKey> : ISnapshot<TAggregateKey, IMockAggregateKeyType>
    {
    }

    public interface IMockAggregateKeyType
    {
    }
}
