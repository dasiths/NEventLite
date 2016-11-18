using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public interface IRepository
    {
        Task<T> GetById<T>(Guid id) where T : AggregateRoot;
        Task Save<T>(T aggregate) where T : AggregateRoot;
    }
}
