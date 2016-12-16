using System.Threading.Tasks;
using NEventLite.Event;

namespace NEventLite.Event_Handler
{
    public interface IEventHandler<T> where T:IEvent
    {
        Task HandleEventAsync(T @event);
    }
}
