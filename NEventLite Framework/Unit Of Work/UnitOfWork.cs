using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Exceptions;
using NEventLite.Repository;

namespace NEventLite.Unit_Of_Work
{
    public class UnitOfWork<T>:IUnitOfWork<T> where T : AggregateRoot, new()
    {
        private readonly IRepository<T> _repository;
        private readonly Dictionary<Guid, T> _trackedAggregates;

        public UnitOfWork(IRepository<T> repository)
        {
            _repository = repository;
            _trackedAggregates = new Dictionary<Guid, T>();
        }

        public void Add(T aggregate)
        {
            if (!IsTracked(aggregate.Id))
                _trackedAggregates.Add(aggregate.Id,aggregate);
            else if (_trackedAggregates[aggregate.Id] != aggregate)
                throw new ConcurrencyException($"Aggregate can't be added because it's already tracked.");
        }

        public T Get(Guid id, int? expectedVersion = null)
        {

            T aggregate = null;
            bool mustbeAdded = false;

            if (IsTracked(id))
            {
                aggregate = _trackedAggregates[id];
            }
            else
            {
                aggregate = _repository.GetById(id);
                mustbeAdded = true;
            }

            if (expectedVersion != null && aggregate.CurrentVersion != expectedVersion)
                throw new ConcurrencyException(
                    $"The aggregate version ({aggregate.CurrentVersion}) doesn't match the expected version ({expectedVersion})");

            if (mustbeAdded)
                Add(aggregate);

            return aggregate;
        }

        private bool IsTracked(Guid id)
        {
            return _trackedAggregates.ContainsKey(id);
        }

        public void Commit()
        {
            foreach (var aggregate in _trackedAggregates.Values)
            {
                _repository.Save(aggregate);
            }
            _trackedAggregates.Clear();
        }

    }
}
