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
    public class InMemoryEventStorageProvider<TEventKey, TAggregate, TAggregateKey> : IEventStorageProvider<TEventKey, TAggregate, TAggregateKey>
        where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        private readonly string _memoryDumpFile;
        private readonly Dictionary<TAggregateKey, List<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>> _eventStream = 
            new Dictionary<TAggregateKey, List<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>>();

        public InMemoryEventStorageProvider(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _eventStream = SerializerHelper.LoadListFromFile<
                    Dictionary<TAggregateKey, List<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>>
                >(_memoryDumpFile).First();
            }
        }

        public Task<IEnumerable<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>> GetEventsAsync(TAggregateKey aggregateId, int start, int count)
        {
            try
            {
                IEnumerable<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>> result = null;

                if (_eventStream.ContainsKey(aggregateId))
                {

                    //this is needed for make sure it doesn't fail when we have int.maxValue for count
                    if (count > int.MaxValue - start)
                    {
                        count = int.MaxValue - start;
                    }

                    result = _eventStream[aggregateId].Where(
                            o =>
                                (_eventStream[aggregateId].IndexOf(o) >= start) &&
                                (_eventStream[aggregateId].IndexOf(o) < (start + count)))
                            .ToArray();
                }
                else
                {
                    result = new List<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>>();
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found. Details {ex.Message}");
            }

        }

        public Task<IEvent<TEventKey, AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey>> GetLastEventAsync(TAggregateKey aggregateId)
        {
            if (_eventStream.ContainsKey(aggregateId))
            {
                return Task.FromResult(_eventStream[aggregateId].Last());
            }

            return Task.FromResult((IEvent<TEventKey, AggregateRoot < TAggregateKey, TEventKey >, TAggregateKey >)null);
        }

        public Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate)
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

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] { _eventStream });
            return Task.CompletedTask;
        }
    }
}
