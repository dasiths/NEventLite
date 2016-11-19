using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;

namespace NEventLite.Event_Handlers
{
    public interface IEventHandler<T> where T:IEvent
    {
        Task HandleEventAsync(T @event);
    }
}
