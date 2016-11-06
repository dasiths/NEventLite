using NEventLite.Repository;
using NEventLite.Session;
using NEventLite.Storage;
using NEventLite.Unit_Of_Work;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Unit_Of_Work
{
    public class MyUnitOfWork : UnitOfWork
    {
        readonly public Repository<Note> NoteRepository;

        public MyUnitOfWork(IEventStorageProvider eventProvider, ISnapshotStorageProvider snapshotProvider) : base(eventProvider,snapshotProvider)
        {
            NoteRepository = new Repository<Note>(Context);
        }

    }
}
