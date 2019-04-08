using System;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Repository
{
    public interface IRepository<TAggregate> : IRepository<TAggregate, Guid, Guid> where TAggregate : AggregateRoot<Guid, Guid>, new()
    {
    }

    public interface IRepository<TAggregate, in TAggregateKey, TEventKey> where TAggregate: AggregateRoot<TAggregateKey, TEventKey>, new()
    {
        Task<TAggregate> GetByIdAsync(TAggregateKey id);
        Task SaveAsync(TAggregate aggregate);
    }
}
