using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp
{
    public class EventPublisher<TEventKey, TAggregateKey>: IEventPublisher<TEventKey, TAggregateKey>
    {
        public Task PublishAsync(IEvent<TEventKey, TAggregateKey> @event)
        {
            var jsonString = JsonConvert.SerializeObject(@event, Formatting.Indented);

            Console.WriteLine($"Event: {@event.TargetVersion + 2}");
            Console.WriteLine(jsonString);
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
