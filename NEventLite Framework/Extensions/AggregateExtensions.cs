using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Exceptions;
using NEventLite.Repository;

namespace NEventLite.Extensions
{
    public static class AggregateExtensions
    {
        public static void EnsureDoesntExist<T>(this IRepository<T> repository, Guid id) where T : AggregateRoot, new()
        {
            var aggregate = repository.GetById(id);

            if (aggregate != null)
            {
                throw new AggregateNotFoundException($"{aggregate.GetType()} with ID {id} already exists.");
            }
        }

        public static T EnsureExists<T>(this IRepository<T> repository, Guid id) where T:AggregateRoot, new()
        {
            var aggregate = repository.GetById(id);

            if (aggregate == null)
            {
                throw new AggregateNotFoundException($"{typeof(T)} with ID {id} was not found.");
            }

            return aggregate;
        }

        public static void EnsureVersionMatch(this AggregateRoot aggregate, int version)
        {
            if (aggregate.CurrentVersion != version)
            {
                throw new ConcurrencyException($"The version of the Note ({aggregate.CurrentVersion})" +$" and Command ({version}) didn't match.");
            }
        }
    }
}
