using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Tests.Integration.Mocks
{
    public class MockEventPublisher<TAggregate> : MockEventPublisher<TAggregate, Guid, Guid> where TAggregate : AggregateRoot<Guid, Guid>
    {
    }

    public class MockEventPublisher<TAggregate, TAggregateKey, TEventKey> : IEventPublisher<TAggregate, TAggregateKey, TEventKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        public readonly Dictionary<TAggregateKey, IList<IEvent<TAggregate, TAggregateKey, TEventKey>>> Events =
            new Dictionary<TAggregateKey, IList<IEvent<TAggregate, TAggregateKey, TEventKey>>>();

        public Task PublishAsync(IEvent<TAggregate, TAggregateKey, TEventKey> @event)
        {
            var list = Events.ContainsKey(@event.AggregateId) ? Events[@event.AggregateId].ToList() :
                new List<IEvent<TAggregate, TAggregateKey, TEventKey>>();

            list.Add(@event);
            Events[@event.AggregateId] = list;
            return Task.CompletedTask;
        }
    }
}
