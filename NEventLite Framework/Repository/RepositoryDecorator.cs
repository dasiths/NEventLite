using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public abstract class RepositoryDecorator:IRepository
    {
        protected readonly IRepository Repository;

        protected RepositoryDecorator(IRepository  repository)
        {
            Repository = repository;
        }

        public virtual TAggregate GetById<TAggregate>(Guid Id) where TAggregate:AggregateRoot
        {
            return Repository.GetById<TAggregate>(Id);
        }

        public virtual void Save<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot
        {
            Repository.Save(aggregate);
        }

    }
}
