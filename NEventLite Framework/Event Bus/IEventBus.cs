using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;

namespace NEventLite.Event_Bus
{
    public interface IEventBus
    {
        void Publish(IEnumerable<IEvent> events);
    }
}
