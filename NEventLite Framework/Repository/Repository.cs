using System;
using System.Linq;
using NEventLite.Domain;
using NEventLite.Session;
using NEventLite.Snapshot;

namespace NEventLite.Repository
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {

        public ISession Session { get; }

        public Repository(Session.ISession context)
        {
            Session = context;
        }

        public void Add(T aggregate)
        {
            Session.Add(aggregate);
        }

        public void Delete(T aggregate)
        {
            throw new NotImplementedException();
        }

        public T GetById(Guid id)
        {

            T item = null;
            var snapshot = Session.SnapshotStorageProvider.GetSnapshot(typeof(T), id);

            if (snapshot != null)
            {
                item = new T();
                ((ISnapshottable)item).SetSnapshot(snapshot);
                var events = Session.EventStorageProvider.GetEvents(typeof(T),id, snapshot.Version + 1, int.MaxValue);
                item.LoadsFromHistory(events);
                Session.Add(item);
            }
            else
            {
                var events = Session.EventStorageProvider.GetEvents(typeof(T),id, 0, int.MaxValue);

                if (events.Any())
                {
                    item = new T();
                    item.LoadsFromHistory(events);
                    Session.Add(item);
                }
            }

            return item;
        }


    }
}
