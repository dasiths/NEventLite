using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Event_Bus;

namespace NEventLite_Example.Event_Bus
{
    public class InMemoryEventBus:IEventBus
    {
        public void Publish(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Console.WriteLine($"Published: {e.GetType().Name}");
            }
        }
    }
}
