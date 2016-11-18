using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Exceptions;
using NEventLite.Storage;

namespace NEventLite_Storage_Providers.InMemory
{
    public class InMemoryEventStorageProvider : IEventStorageProvider
    {
        private Dictionary<Guid, List<IEvent>> _eventStream = new Dictionary<Guid, List<IEvent>>();

        public async Task<IEnumerable<IEvent>> GetEventsAsync(Type aggregateType, Guid aggregateId, int start, int count)
        {
            try
            {
                if (_eventStream.ContainsKey(aggregateId))
                {

                    //this is needed for make sure it doesn't fail when we have int.maxValue for count
                    if (count > int.MaxValue - start)
                    {
                        count = int.MaxValue - start;
                    }

                    return
                        _eventStream[aggregateId].Where(
                            o =>
                                (_eventStream[aggregateId].IndexOf(o) >= start) &&
                                (_eventStream[aggregateId].IndexOf(o) < (start + count)))
                            .ToArray();
                }
                else
                {
                    return new List<IEvent>();
                }
                
            }
            catch (Exception ex)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found. Details {ex.Message}");
            }

        }

        public async Task<IEvent> GetLastEventAsync(Type aggregateType, Guid aggregateId)
        {
            if (_eventStream.ContainsKey(aggregateId))
            {
                return _eventStream[aggregateId].Last();
            }
            else
            {
                return null;
            }
        }

        public async Task CommitChangesAsync(AggregateRoot aggregate)
        {
            var events = aggregate.GetUncommittedChanges();

            if (events.Any())
            {
                if (_eventStream.ContainsKey(aggregate.Id) == false)
                {
                    _eventStream.Add(aggregate.Id, events.ToList());
                }
                else
                {
                    _eventStream[aggregate.Id].AddRange(events);
                }
            }

        }
    }
}
