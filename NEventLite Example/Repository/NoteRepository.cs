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
    public class NoteRepository:MyRepositoryDecorator<Note>
    {
        private DateTime _commitStartTime;

        public NoteRepository(IRepository<Note> repository) : base(repository)
        {
            
        }

        public override void BeforeLoadAggregate(Guid id)
        {
            base.BeforeLoadAggregate(id);
        }

        public override void AfterLoadingAggregate(Note aggregate)
        {
            base.AfterLoadingAggregate(aggregate);
        }

        public override void BeforeSaveAggregate(Note aggregate)
        {
            _commitStartTime = DateTime.Now;
            Console.WriteLine($"Trying to commit {aggregate.GetUncommittedChanges().Count()} events to storage.");
            base.BeforeSaveAggregate(aggregate);
        }

        public override void AfterSavingAggregate(Note aggregate)
        {
            Console.WriteLine($"Committed in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.");
            base.AfterSavingAggregate(aggregate);
        }
    }
}
