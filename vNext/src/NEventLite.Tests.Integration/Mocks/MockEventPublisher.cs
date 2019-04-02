using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Tests.Integration.Mocks
{
    public class MockEventPublisher<TEventKey, TAggregate, TAggregateKey> : IEventPublisher<TEventKey, TAggregate, TAggregateKey> 
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        public readonly Dictionary<TAggregateKey, IList<IEvent<TEventKey, TAggregate, TAggregateKey>>> Events = 
            new Dictionary<TAggregateKey, IList<IEvent<TEventKey, TAggregate, TAggregateKey>>>();

        public Task PublishAsync(IEvent<TEventKey, TAggregate, TAggregateKey> @event)
        {
            var list = Events.ContainsKey(@event.AggregateId) ? Events[@event.AggregateId].ToList() :
                new List<IEvent<TEventKey, TAggregate, TAggregateKey>>();

            list.Add(@event);
            Events[@event.AggregateId] = list;
            return Task.CompletedTask;
        }
    }
}
