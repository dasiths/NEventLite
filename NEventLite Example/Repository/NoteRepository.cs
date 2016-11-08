using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Logger;
using NEventLite.Repository;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Repository
{
    public class NoteRepository:MyRepositoryDecorator<Note>
    {
        public NoteRepository(IRepository<Note> repository) : base(repository)
        {
            
        }

        public override void Save(Note aggregate)
        {
            LogManager.Log("Saving Note...", LogSeverity.Information);
            base.Save(aggregate);
            LogManager.Log("Note Saved...", LogSeverity.Information);
        }
    }
}
