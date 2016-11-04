using System;
using System.Collections.Generic;
using NEventLite.Domain;

namespace NEventLite.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Add(T aggregate);
        void Delete(T aggregate);
        T GetById(Guid id);
    }
}
