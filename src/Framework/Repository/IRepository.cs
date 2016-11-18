using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public interface IRepository
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot;
        Task SaveAsync<T>(T aggregate) where T : AggregateRoot;
    }
}
