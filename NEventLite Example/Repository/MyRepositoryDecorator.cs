using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Repository;

namespace NEventLite_Example.Repository
{
    public class MyRepositoryDecorator<T>:RepositoryDecorator<T> where T : AggregateRoot, new()
    {
        private DateTime _commitStartTime;

        public MyRepositoryDecorator(IRepository<T> repository) : base(repository)
        {
        }

        public override T GetById(Guid Id)
        {
            BeforeLoadAggregate(Id);
            var result =  base.GetById(Id);
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Loading {id} ...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        protected void AfterLoadingAggregate(T aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Loaded {aggregate.GetType()} ...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        protected void BeforeSaveAggregate(T aggregate)
        {
            _commitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.");
        }

        protected void AfterSavingAggregate(T aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Committed in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
