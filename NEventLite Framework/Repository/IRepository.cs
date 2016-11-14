using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public interface IRepository
    {
        T GetById<T>(Guid id) where T:AggregateRoot;
        void Save<T>(T aggregate) where T:AggregateRoot;
    }
}
