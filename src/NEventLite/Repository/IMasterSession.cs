using System;
using System.Threading.Tasks;
using NEventLite.Core.Domain;

namespace NEventLite.Repository
{
    public interface IMasterSession : IDisposable
    {
        Task<TAggregate> GetByIdAsync<TAggregate>(object id) where TAggregate : IAggregateRoot;
        void Attach<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot;
        void Detach<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot;
        Task SaveAsync();
        void DetachAll();
    }
}