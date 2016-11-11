using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;

namespace NEventLite.Unit_Of_Work
{
    public interface IUnitOfWork<T> where T:AggregateRoot, new()
    {
        T Get(Guid id, int? expectedVersion = null);
        void Add(T aggregate);
        void Commit();
    }
}
