using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Core
{
    public interface IEventPublisher<in TAggregate, TAggregateKey, TEventKey> where TAggregate: AggregateRoot<TAggregateKey, TEventKey>
    {
        Task PublishAsync(IEvent<TAggregate, TAggregateKey, TEventKey> @event);
    }
}
