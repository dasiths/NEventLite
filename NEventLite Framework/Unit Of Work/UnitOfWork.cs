using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Repository;
using NEventLite.Storage;

namespace NEventLite.Unit_Of_Work
{
    public abstract class UnitOfWork:IUnitOfWork
    {
        protected ChangeTrackingContext _context;

        protected UnitOfWork(ChangeTrackingContext changeTrackingContext)
        {
            _context = changeTrackingContext;
        }

        public void Commit()
        {
            _context.CommitChanges();
        }
    }
}
