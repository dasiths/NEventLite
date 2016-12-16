using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Event;
using NEventLite.Event_Bus;
using NEventLite.Extension;
using NEventLite.Logger;
using NEventLite_Example.Read_Model;

namespace NEventLite_Example.Event_Bus
{
    public class MyEventPublisher : IEventPublisher
    {
        private readonly MyEventSubscriber _subscriber;

        public MyEventPublisher(MyEventSubscriber subscriber)
        {
            LogManager.Log("EventPublisher Started...", LogSeverity.Information);
            _subscriber = subscriber;
        }

        public async Task PublishAsync(IEvent @event)
        {
            LogManager.Log(
                $"Event #{@event.TargetVersion + 1} Published: {@event.GetType().Name} @ {DateTime.Now.ToLongTimeString()}",
                LogSeverity.Information);

            await Task.Run(() =>
            {
                InvokeSubscriber(@event);

            }).ConfigureAwait(false);
        }

        private void InvokeSubscriber<T>(T @event) where T : IEvent
        {
            var o = _subscriber.GetType().GetMethodsBySig(typeof(Task), null, true, @event.GetType()).First();
            o.Invoke(_subscriber, new object[] { @event });
        }
    }
}
