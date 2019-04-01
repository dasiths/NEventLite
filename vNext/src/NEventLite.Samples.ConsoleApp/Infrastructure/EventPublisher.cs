using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp.Infrastructure
{
    public class EventPublisher<TEventKey, TAggregateKey>: IEventPublisher<TEventKey, TAggregateKey>
    {
        public Task PublishAsync(IEvent<TEventKey, TAggregateKey> @event)
        {
            var jsonString = JsonConvert.SerializeObject(@event, Formatting.Indented);
            Console.WriteLine(jsonString);

            return Task.CompletedTask;
        }
    }
}
