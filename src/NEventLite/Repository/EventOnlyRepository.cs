using System;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class EventOnlyRepository<TAggregate> : 
        EventOnlyRepository<TAggregate, Guid, Guid>,
        IRepository<TAggregate> 
        where TAggregate : AggregateRoot<Guid, Guid>, new()
    {
        public EventOnlyRepository(IClock clock, IEventStorageProvider<Guid, Guid> eventStorageProvider, IEventPublisher eventPublisher) : 
            base(clock, eventStorageProvider, eventPublisher)
        {
        }

        public EventOnlyRepository(IClock clock, IEventStorageProvider eventStorageProvider, IEventPublisher eventPublisher) :
            base(clock, eventStorageProvider, eventPublisher)
        {
        }
    }

    public class EventOnlyRepository<TAggregate, TAggregateKey, TEventKey> : Repository<TAggregate, IMockSnapShot<TAggregateKey>, TAggregateKey, TEventKey, IMockAggregateKeyType>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        public EventOnlyRepository(IClock clock,
            IEventStorageProvider<TAggregateKey, TEventKey> eventStorageProvider,
            IEventPublisher eventPublisher) : base(clock, eventStorageProvider, eventPublisher, null)
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
