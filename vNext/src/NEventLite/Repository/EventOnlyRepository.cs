using System;
using System.Collections.Generic;
using System.Text;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class EventOnlyRepository<TAggregate, TAggregateKey, TEventKey> : Repository<TAggregate, TAggregateKey, TEventKey, IMockAggregateKeyType, IMockSnapShot<TAggregateKey>>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        public EventOnlyRepository(IClock clock,
            IEventStorageProvider<TEventKey, TAggregateKey> eventStorageProvider,
            IEventPublisher<TEventKey, TAggregateKey> eventPublisher) : base(clock, eventStorageProvider, eventPublisher, null)
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
