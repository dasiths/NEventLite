using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Tests.Integration.Mocks
{
    public class MockEventPublisher<TEventKey, TAggregateKey> : IEventPublisher<TEventKey, TAggregateKey>
    {
        public readonly Dictionary<TAggregateKey, IList<IEvent<TEventKey, TAggregateKey>>> Events = 
            new Dictionary<TAggregateKey, IList<IEvent<TEventKey, TAggregateKey>>>();

        public Task PublishAsync(IEvent<TEventKey, TAggregateKey> @event)
        {
            var list = Events.ContainsKey(@event.AggregateId) ? Events[@event.AggregateId].ToList() :
                new List<IEvent<TEventKey, TAggregateKey>>();

            list.Add(@event);
            Events[@event.AggregateId] = list;
            return Task.CompletedTask;
        }
    }
}
