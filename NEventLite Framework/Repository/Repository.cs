using System;
using System.Collections.Generic;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Exceptions;
using NEventLite.Snapshot;
using NEventLite.Storage;

namespace NEventLite.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private ChangeTrackingContext _context;
       
        public Repository(ChangeTrackingContext changeTrackingContext)
        {
            _context = changeTrackingContext;
        }

        public void Add(T aggregate)
        {
            aggregate.SetTracker(_context);
        }

        public void Delete(T aggregate)
        {
            throw new NotImplementedException();
        }

        public T GetById(Guid id)
        {

            T item = null;
            var snapshot = _context.SnapshotStorageProvider.GetSnapshot(typeof(T), id);

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = _context.EventStorageProvider.GetEvents(typeof(T),id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
                item.SetTracker(_context);
            }
            else
            {
                var events = _context.EventStorageProvider.GetEvents(typeof(T),id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = new T();
                    item.LoadsFromHistory(events);
                    item.SetTracker(_context);
                }
            }

            return item;
        }


    }
}
