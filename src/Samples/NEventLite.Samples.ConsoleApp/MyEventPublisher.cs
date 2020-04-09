using System;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp
{
    public class MyEventPublisher : IEventPublisher
    {
        public Task PublishAsync<TAggregate, TAggregateKey, TEventKey>(IEvent<TAggregate, TAggregateKey, TEventKey> @event) where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            Console.WriteLine($"Event {@event.TargetVersion + 2}");
            Program.PrintToConsole(@event, ConsoleColor.Cyan);
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
