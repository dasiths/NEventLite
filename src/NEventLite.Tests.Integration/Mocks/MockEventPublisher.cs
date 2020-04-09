using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Tests.Integration.Mocks
{
    public class MockEventPublisher : IEventPublisher
    {
        public readonly Dictionary<object, IList<IEvent>> Events = new Dictionary<object, IList<IEvent>>();

        public Task PublishAsync<TAggregate, TAggregateKey, TEventKey>(IEvent<TAggregate, TAggregateKey, TEventKey> @event) where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            var list = Events.ContainsKey(@event.AggregateId) ? Events[@event.AggregateId].ToList() :
                new List<IEvent>();

            list.Add(@event);
            Events[@event.AggregateId] = list;
            return Task.CompletedTask;
        }
    }
}
