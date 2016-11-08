using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public abstract class RepositoryDecorator<TAggregate>:IRepository<TAggregate> 
        where TAggregate:AggregateRoot, new() 
    {
        protected readonly IRepository<TAggregate> Repository;

        protected RepositoryDecorator(IRepository<TAggregate>  repository)
        {
            Repository = repository;
        }

        public virtual TAggregate GetById(Guid Id)
        {
            return Repository.GetById(Id);
        }

        public virtual void Save(TAggregate aggregate)
        {
            Repository.Save(aggregate);
        }

    }
}
