using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Repository;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Repository
{
    public class NoteRepository:RepositoryDecorator<Note>
    {
        private DateTime _commitStartTime;

        public NoteRepository(IRepositoryBase<Note> repositoryBase) : base(repositoryBase)
        {
            
        }

        public override void Save(Note aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            base.Save(aggregate);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override void BeforeLoadAggregate(Guid id)
        {
            
        }

        public override void AfterLoadingAggregate(Note aggregate)
        {
            
        }

        public override void BeforeSaveAggregate(Note aggregate)
        {
            _commitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.");
        }

        public override void AfterSavingAggregate(Note aggregate)
        {
            Console.WriteLine($"Committed in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.");
        }
    }
}
