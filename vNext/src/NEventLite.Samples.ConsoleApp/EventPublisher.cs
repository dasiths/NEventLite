using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp
{
    public class EventPublisher<TAggregate, TAggregateKey, TEventKey> : IEventPublisher<TAggregate, TAggregateKey, TEventKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        public Task PublishAsync(IEvent<TAggregate, TAggregateKey, TEventKey> @event)
        {
            var jsonString = JsonConvert.SerializeObject(@event, Formatting.Indented);

            Console.WriteLine($"Event: {@event.TargetVersion + 2}");
            Console.WriteLine(jsonString);
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
