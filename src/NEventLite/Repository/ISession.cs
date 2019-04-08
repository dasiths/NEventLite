
using System;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Repository
{
    public interface ISession<TAggregate> : ISession<TAggregate, Guid, Guid> 
        where TAggregate : AggregateRoot<Guid, Guid>, new()
    {
    }

    public interface ISession<TAggregate, in TAggregateKey, TEventKey> : IDisposable
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        Task<TAggregate> GetByIdAsync(TAggregateKey id);
        void Attach(TAggregate aggregate);
        void Detach(TAggregate aggregate);
        Task SaveAsync();
        void DetachAll();
    }
}
