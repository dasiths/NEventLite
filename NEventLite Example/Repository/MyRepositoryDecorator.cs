using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;
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
            AfterLoadingAggregate(Id, result);
            return result;
        }

        public override void Save(T aggregate)
        {
            var events = aggregate.GetUncommittedChanges().ToList();

            BeforeSaveAggregate(aggregate, events);
            base.Save(aggregate);
            AfterSavingAggregate(aggregate, events);
        }

        protected void BeforeLoadAggregate(Guid id)
        {
            LogManager.Log($"Loading {id} ...", LogSeverity.Debug);
        }

        protected void AfterLoadingAggregate(Guid id, T aggregate)
        {
            if (aggregate != null)
            {
                LogManager.Log($"Loaded {aggregate.GetType()}", LogSeverity.Debug);
            }
            else
            {
                LogManager.Log($"Aggregate {id} not found.", LogSeverity.Warning);
            }
            
        }

        protected void BeforeSaveAggregate(T aggregate, IEnumerable<IEvent> events)
        {
            _commitStartTime = DateTime.Now;
            LogManager.Log($"Trying to commit {events.Count()} event(s) to storage.", LogSeverity.Debug);
        }

        protected void AfterSavingAggregate(T aggregate, IEnumerable<IEvent> events)
        {
            LogManager.Log($"Committed {events.Count()} event(s) in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.", LogSeverity.Debug);
        }
    }
}
