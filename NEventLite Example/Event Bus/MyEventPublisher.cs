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
    public class MyEventPublisher : IEventPublisher
    {
        public MyEventPublisher()
        {
            LogManager.Log("EventPublisher Started...", LogSeverity.Information);
        }

        public Task Publish(IEnumerable<IEvent> events)
        {
            return Task.Run(() =>
            {
                foreach (var e in events)
                {
                    LogManager.Log(
                        $"Event #{e.TargetVersion + 1} Published: {e.GetType().Name} @ {DateTime.Now.ToLongTimeString()}", 
                        LogSeverity.Information);
                }
            });
        }
    }
}
