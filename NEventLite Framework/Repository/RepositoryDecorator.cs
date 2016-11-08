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
            BeforeLoadAggregate(Id);
            var result =  Repository.GetById(Id);
            AfterLoadingAggregate(result);
            return result;
        }

        public virtual void Save(TAggregate aggregate)
        {
            BeforeSaveAggregate(aggregate);
            Repository.Save(aggregate);
            AfterSavingAggregate(aggregate);
        }

        public abstract void BeforeLoadAggregate(Guid id);

        public abstract void AfterLoadingAggregate(TAggregate aggregate);

        public abstract void BeforeSaveAggregate(TAggregate aggregate);

        public abstract void AfterSavingAggregate(TAggregate aggregate);
    }
}
