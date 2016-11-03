using System;
using NEventLite.Domain;

namespace NEventLite.Repository
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate);
        T GetById(Guid id);
    }
}
