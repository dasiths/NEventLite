using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using EventSourcingDemo.Exceptions;
using EventSourcingDemo.Storage;

namespace EventSourcingDemo.Repository
{
    class Repository<T>:IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStorageProvider _storageProvider;
        private static object syncLockObject = new object();

        public void Save(AggregateRoot aggregate)
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                lock (syncLockObject)
                {
                    var item = new T();
                    var expectedVersion = aggregate.LastCommitedVersion;

                    if (expectedVersion != 0)
                    {
                        item = GetById(aggregate.Id);
                        if (item.CurrentVersion != expectedVersion)
                        {
                            throw new ConcurrencyException($"Aggregate {item.Id} has been modified externally and has an updated state. Can't commit changes.");
                        }
                    }

                    _storageProvider.CommitChanges(aggregate);
                    aggregate.MarkChangesAsCommitted();
                }
            }
        }

        public T GetById(Guid id)
        {
            var events = _storageProvider.GetEvents(id, 0, int.MaxValue);

            var item = new T();
            item.LoadsFromHistory(events);

            return item;
        }
    }
}
