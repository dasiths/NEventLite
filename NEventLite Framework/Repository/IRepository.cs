using System;
using NEventLite.Domain;

namespace NEventLite.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(T aggregate);
    }
}
