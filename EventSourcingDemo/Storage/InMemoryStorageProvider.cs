using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Events;
using EventSourcingDemo.Exceptions;

namespace EventSourcingDemo.Storage
{
    class InMemoryStorageProvider : IEventStorageProvider
    {
        private Dictionary<Guid, List<Event>> eventStream = new Dictionary<Guid, List<Event>>();

        public IEnumerable<Event> GetEvents(Guid aggregateId, int start, int end)
        {
            try
            {
                return
                    eventStream[aggregateId].Where(
                        o => (eventStream[aggregateId].IndexOf(o) >= start) && (eventStream[aggregateId].IndexOf(o) < end))
                        .ToArray();
            }
            catch (Exception)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found.");
            }

        }

        public void CommitChanges(AggregateRoot aggregate)
        {
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                if (eventStream.ContainsKey(aggregate.Id) == false)
                {
                    eventStream.Add(aggregate.Id, events.ToList());
                }
                else
                {
                    eventStream[aggregate.Id].AddRange(events);
                }
            }


        }
    }
}
