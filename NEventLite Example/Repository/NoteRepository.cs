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
            repositoryBase.PreCommitActions.Add(o =>
            {
                _commitStartTime = DateTime.Now;
                Console.WriteLine($"Trying to commit {o.GetUncommittedChanges().Count()} events to storage.");
            });

            repositoryBase.PostCommitActions.Add((aggregate,events) =>
            {
                Console.WriteLine($"Committed {events.Count()} events to storage in {DateTime.Now.Subtract(_commitStartTime).TotalMilliseconds} ms.");
            });
        }

        public override void Save(Note aggregate)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            base.Save(aggregate);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
