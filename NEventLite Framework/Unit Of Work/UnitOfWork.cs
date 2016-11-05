namespace NEventLite.Unit_Of_Work
{
    public abstract class UnitOfWork:IUnitOfWork
    {
        protected Session.ISession _context;

        protected UnitOfWork(Session.ISession session)
        {
            _context = session;
        }

        public void Commit()
        {
            _context.CommitChanges();
        }
    }
}
