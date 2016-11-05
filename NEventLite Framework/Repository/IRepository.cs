using System;
using NEventLite.Domain;
using NEventLite.Session;

namespace NEventLite.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        ISession Session { get;}
        void Add(T aggregate);
        void Delete(T aggregate);
        T GetById(Guid id);
    }
}
