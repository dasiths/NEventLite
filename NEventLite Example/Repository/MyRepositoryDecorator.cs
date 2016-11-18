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
    public class MyRepositoryDecorator : RepositoryDecorator
    {
        private DateTime _commitStartTime;

        public MyRepositoryDecorator(IRepository repository) : base(repository)
        {
        }

        public override async Task<T> GetByIdAsync<T>(Guid Id)
        {
            BeforeLoadAggregate(Id);
            var result = await base.GetByIdAsync<T>(Id);
            AfterLoadingAggregate(Id, result);
            return result;
        }

        public override async Task SaveAsync<T>(T aggregate)
        {
            var events = aggregate.GetUncommittedChanges().ToList();

            BeforeSaveAggregate(aggregate, events);
            await base.SaveAsync(aggregate);
            AfterSavingAggregate(aggregate, events);
        }

        protected void BeforeLoadAggregate(Guid id)
        {
            LogManager.Log($"Loading {id} ...", LogSeverity.Debug);
        }

        protected void AfterLoadingAggregate<T>(Guid id, T aggregate)
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

        protected void BeforeSaveAggregate<T>(T aggregate, IEnumerable<IEvent> events)
        {
            _commitStartTime = DateTime.Now;
            LogManager.Log($"Trying to commit {events.Count()} event(s) to storage.", LogSeverity.Debug);
        }

        protected void AfterSavingAggregate<T>(T aggregate, IEnumerable<IEvent> events)
        {
            LogManager.Log($"Committed {events.Count()} event(s) in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.", LogSeverity.Debug);
        }
    }
}
