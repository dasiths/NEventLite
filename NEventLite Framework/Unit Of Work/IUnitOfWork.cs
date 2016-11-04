using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
