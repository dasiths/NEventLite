using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite.Unit_Of_Work;
using NEventLite_Example.Domain;
using NEventLite_Example.Util;

namespace NEventLite_Example.Unit_Of_Work
{
    public class MyUnitOfWork : UnitOfWork
    {
        readonly public Repository<Note> NoteRepository;

        public MyUnitOfWork(ChangeTrackingContext changeTrackingContext) : base(changeTrackingContext)
        {
            NoteRepository = new Repository<Note>(changeTrackingContext);
            changeTrackingContext.SnapshotFrequency = 5;
        }

    }
}
