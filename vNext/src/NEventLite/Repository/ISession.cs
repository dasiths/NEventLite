
using System;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Repository
{
    public interface ISession<TAggregate, in TAggregateKey, TEventKey> : IDisposable
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        Task<TAggregate> GetByIdAsync(TAggregateKey id);
        void Attach(TAggregate aggregate);
        Task CommitChangesAsync();
    }
}
