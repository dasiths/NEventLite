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
    public class NoteRepository:MyRepositoryDecoratorBase<Note>
    {
        public NoteRepository(IRepository<Note> repository) : base(repository)
        {
            
        }
        
        public override void BeforeSaveAggregate(Note aggregate)
        {
            LogManager.Logger.Log("Saving Note...",LogSeverity.Information);
            base.BeforeSaveAggregate(aggregate);
        }

        public override void AfterSavingAggregate(Note aggregate)
        {
            base.AfterSavingAggregate(aggregate);
            LogManager.Logger.Log("Note Saved...", LogSeverity.Information);
        }
    }
}
