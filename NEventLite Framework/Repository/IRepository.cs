using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(T aggregate);
    }
}
