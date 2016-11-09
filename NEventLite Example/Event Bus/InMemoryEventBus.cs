using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Event_Bus;
using NEventLite.Logger;

namespace NEventLite_Example.Event_Bus
{
    public class InMemoryEventBus : IEventBus
    {
        public InMemoryEventBus()
        {
            LogManager.Log("EventBus Started...", LogSeverity.Information);
        }

        public void Publish(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                LogManager.Log($"Published: {e.GetType().Name}", LogSeverity.Information);
            }
        }
    }
}
