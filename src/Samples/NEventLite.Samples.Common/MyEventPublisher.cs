using System;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using Newtonsoft.Json;

namespace NEventLite.Samples.Common
{
    public class MyEventPublisher : IEventPublisher
    {
        public Task PublishAsync<TAggregate, TAggregateKey, TEventKey>(IEvent<TAggregate, TAggregateKey, TEventKey> @event) where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            Console.WriteLine($"Event {@event.TargetVersion + 2}");
            PrintToConsole(@event, ConsoleColor.Cyan);
            Console.WriteLine();

            return Task.CompletedTask;
        }

        public static void PrintToConsole(object @object, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(JsonConvert.SerializeObject(@object, Formatting.Indented));
            Console.ResetColor();
        }
    }
}
