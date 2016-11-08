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
        public MyRepositoryDecorator(IRepository<T> repository) : base(repository)
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
           
        }

        public override void BeforeSaveAggregate(T aggregate)
        {
            
        }

        public override void AfterSavingAggregate(T aggregate)
        {
            
        }
    }
}
