using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Core
{
    public interface IEventPublisher<TEventKey, in TAggregate, TAggregateKey> where TAggregate: AggregateRoot<TAggregateKey, TEventKey>
    {
        Task PublishAsync(IEvent<TEventKey, TAggregate, TAggregateKey> @event);
    }
}
