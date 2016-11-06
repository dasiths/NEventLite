using System.Linq.Expressions;
using NEventLite.Storage;

namespace NEventLite.Unit_Of_Work
{
    public abstract class UnitOfWork:IUnitOfWork
    {
        protected Session.ISession Context;

        protected UnitOfWork(IEventStorageProvider eventProvider, ISnapshotStorageProvider snapshotProvider)
        {
            Context = new Session.Session(eventProvider,snapshotProvider);
        }

        public void Commit()
        {
            Context.CommitChanges();
        }
    }
}
