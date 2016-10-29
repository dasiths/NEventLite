using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;

namespace EventSourcingDemo.Event_Handlers
{
    public interface IEventHandler<T> where T: Events.Event
    {
        void Apply(T @event);
    }
}
