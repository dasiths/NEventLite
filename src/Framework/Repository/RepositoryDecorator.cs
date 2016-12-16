using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;

namespace NEventLite.Repository
{
    public abstract class RepositoryDecorator:IRepository
    {
        protected readonly IRepository Repository;

        protected RepositoryDecorator(IRepository  repository)
        {
            Repository = repository;
        }

        public virtual async Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate:AggregateRoot
        {
            return await Repository.GetByIdAsync<TAggregate>(id);
        }

        public virtual async Task SaveAsync<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot
        {
            await Repository.SaveAsync(aggregate);
        }

    }
}
