using System;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp
{
    public class EventPublisher<TAggregate> : EventPublisher<TAggregate, Guid, Guid> where TAggregate : AggregateRoot<Guid, Guid>
    {
    }

    public class EventPublisher<TAggregate, TAggregateKey, TEventKey> : IEventPublisher<TAggregate, TAggregateKey, TEventKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        public Task PublishAsync(IEvent<TAggregate, TAggregateKey, TEventKey> @event)
        {
            Console.WriteLine($"Event {@event.TargetVersion + 2}");
            Program.PrintToConsole(@event, ConsoleColor.Cyan);
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
