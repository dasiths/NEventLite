using System;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Core
{
    public interface IEventPublisher
    {
        Task PublishAsync<TAggregate, TAggregateKey, TEventKey>(IEvent<TAggregate, TAggregateKey, TEventKey> @event) where TAggregate : AggregateRoot<TAggregateKey, TEventKey>;
    }

    public class DefaultNoOpEventPublisher : IEventPublisher
    {
        public Task PublishAsync<TAggregate, TAggregateKey, TEventKey>(IEvent<TAggregate, TAggregateKey, TEventKey> @event) where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            return Task.CompletedTask;
        }
    }
}
