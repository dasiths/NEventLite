using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;

namespace NEventLite.Repository
{
    public abstract class RepositoryDecorator<TAggregate> 
        where TAggregate:AggregateRoot, new() 
    {
        protected readonly IRepositoryBase<TAggregate> RepositoryBase;

        protected RepositoryDecorator(IRepositoryBase<TAggregate>  repositoryBase)
        {
            RepositoryBase = repositoryBase;
        }

        public virtual TAggregate GetById(Guid Id)
        {
            return RepositoryBase.GetById(Id);
        }

        public virtual void Save(TAggregate aggregate)
        {
            RepositoryBase.Save(aggregate);
        }
    }
}
