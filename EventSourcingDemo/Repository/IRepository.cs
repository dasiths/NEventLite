using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;

namespace EventSourcingDemo.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate);
        T GetById(Guid id);
    }
}
