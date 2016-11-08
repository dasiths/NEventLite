using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Repository;

namespace NEventLite_Example.Repository
{
    public class MyRepositoryDecoratorBase<T>:RepositoryDecorator<T> where T : AggregateRoot, new()
    {
        private DateTime _commitStartTime;

        public MyRepositoryDecoratorBase(IRepository<T> repository) : base(repository)
        {
        }

        public override void Save(T aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            base.Save(aggregate);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void BeforeLoadAggregate(Guid id)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Loading {id} ...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void AfterLoadingAggregate(T aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Loaded {aggregate.GetType()} ...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void BeforeSaveAggregate(T aggregate)
        {
            _commitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.");
        }

        public override void AfterSavingAggregate(T aggregate)
        {
            Console.WriteLine($"Committed in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.");
        }
    }
}
