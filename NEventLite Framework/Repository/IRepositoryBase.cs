using System;
using System.Collections.Generic;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Repository
{
    public interface IRepositoryBase<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(T aggregate);

        List<Action<Guid>> PreLoadActions { get; }
        List<Action<T>> PostLoadActions { get;  }

        List<Action<T>> PreCommitActions { get; }
        List<Action<T, IEnumerable<IEvent>>> PostCommitActions{ get; }
    }
}
