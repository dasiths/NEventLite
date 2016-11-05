using NEventLite.Repository;
using NEventLite.Session;
using NEventLite.Unit_Of_Work;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Unit_Of_Work
{
    public class MyUnitOfWork : UnitOfWork
    {
        readonly public Repository<Note> NoteRepository;

        public MyUnitOfWork(Session session) : base(session)
        {
            NoteRepository = new Repository<Note>(session);
        }

    }
}
