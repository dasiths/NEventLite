using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Logger;
using NEventLite.Repository;

namespace NEventLite_Example.Repository
{
    public class MyRepositoryDecorator<T> : RepositoryDecorator<T> where T : AggregateRoot, new()
    {
        private DateTime _commitStartTime;

        public MyRepositoryDecorator(IRepository<T> repository) : base(repository)
        {
        }

        public override T GetById(Guid Id)
        {
            BeforeLoadAggregate(Id);
            var result = base.GetById(Id);
            AfterLoadingAggregate(result);
            return result;
        }

        public override void Save(T aggregate)
        {
            BeforeSaveAggregate(aggregate);
            base.Save(aggregate);
            AfterSavingAggregate(aggregate);
        }

        protected void BeforeLoadAggregate(Guid id)
        {
            LogManager.Log($"Loading {id} ...", LogSeverity.Debug);
        }

        protected void AfterLoadingAggregate(T aggregate)
        {
            LogManager.Log($"Loaded {aggregate.GetType()} ...", LogSeverity.Debug);
        }

        protected void BeforeSaveAggregate(T aggregate)
        {
            _commitStartTime = DateTime.Now;
            LogManager.Log($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.", LogSeverity.Debug);
        }

        protected void AfterSavingAggregate(T aggregate)
        {
            LogManager.Log($"Committed in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.", LogSeverity.Debug);
        }
    }
}
