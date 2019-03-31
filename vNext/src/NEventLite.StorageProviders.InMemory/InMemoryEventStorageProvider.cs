using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core.Domain;
using NEventLite.Exceptions;
using NEventLite.Storage;

namespace NEventLite.StorageProviders.InMemory
{
    public class InMemoryEventStorageProvider<TEventKey, TAggregateKey> : IEventStorageProvider<TEventKey, TAggregateKey>
    {
        private readonly string _memoryDumpFile;
        private readonly Dictionary<TAggregateKey, List<IEvent<TEventKey,TAggregateKey>>> _eventStream = new Dictionary<TAggregateKey, List<IEvent<TEventKey, TAggregateKey>>>();

        public InMemoryEventStorageProvider(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _eventStream = SerializerHelper.LoadListFromFile<
                    Dictionary<TAggregateKey, List<IEvent<TEventKey, TAggregateKey>>>
                >(_memoryDumpFile).First();
            }
        }

        public async Task<IEnumerable<IEvent<TEventKey, TAggregateKey>>> GetEventsAsync(Type aggregateType, TAggregateKey aggregateId, int start, int count)
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
                    return new List<IEvent<TEventKey, TAggregateKey>>();
                }

            }
            catch (Exception ex)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found. Details {ex.Message}");
            }

        }

        public async Task<IEvent<TEventKey, TAggregateKey>> GetLastEventAsync(Type aggregateType, TAggregateKey aggregateId)
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

        public async Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate)
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

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] {_eventStream});

        }
    }
}
