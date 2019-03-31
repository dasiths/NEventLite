using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Core
{
    public interface IEventPublisher<TEventKey, TAggregateKey>
    {
        Task PublishAsync(IEvent<TEventKey, TAggregateKey> @event);
    }
}
