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

        public async Task PublishAsync(IEvent @event)
        {
            await Task.Run(() =>
            {
                    LogManager.Log(
                        $"Event #{@event.TargetVersion + 1} Published: {@event.GetType().Name} @ {DateTime.Now.ToLongTimeString()}", 
                        LogSeverity.Information);
            });
        }
    }
}
