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
    public class NoteRepository:RepositoryDecorator
    {
        public NoteRepository(IRepository repository) : base(repository)
        {
            
        }

        public override void Save<T>(T aggregate)
        {
            LogManager.Log($"Saving {aggregate.GetType().Name}...", LogSeverity.Debug);
            base.Save(aggregate);
            LogManager.Log($"{aggregate.GetType().Name} Saved...", LogSeverity.Debug);
        }
    }
}
