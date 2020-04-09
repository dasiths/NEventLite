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
    public class InMemoryEventStorageProvider : InMemoryEventStorageProvider<Guid, Guid>, IEventStorageProvider 
    {
        public InMemoryEventStorageProvider() : this(string.Empty)
        {
        }

        public InMemoryEventStorageProvider(string memoryDumpFile) : base(memoryDumpFile)
        {
        }
    }

    public class InMemoryEventStorageProvider<TAggregateKey, TEventKey> : IEventStorageProvider<TAggregateKey, TEventKey>
    {
        private readonly string _memoryDumpFile;
        private readonly Dictionary<TAggregateKey, List<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> _eventStream = 
            new Dictionary<TAggregateKey, List<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>>();

        public InMemoryEventStorageProvider() : this(string.Empty)
        {
        }

        public InMemoryEventStorageProvider(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (!string.IsNullOrWhiteSpace(_memoryDumpFile) && File.Exists(_memoryDumpFile))
            {
                _eventStream =
                    SerializerHelper.LoadFromFile(_memoryDumpFile) as 
                        Dictionary<TAggregateKey, List<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>>?? 
                    new Dictionary<TAggregateKey, List<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>>();
            }
        }

        public Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> GetEventsAsync<TAggregate>(TAggregateKey aggregateId, int start, int count)
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            try
            {
                IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> result = null;

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
                    result = new List<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>();
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new AggregateNotFoundException($"The aggregate with {aggregateId} was not found. Details {ex.Message}");
            }

        }

        public Task<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> GetLastEventAsync<TAggregate>(TAggregateKey aggregateId)
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
        {
            if (_eventStream.ContainsKey(aggregateId))
            {
                return Task.FromResult(_eventStream[aggregateId].Last());
            }

            return Task.FromResult((IEvent<AggregateRoot < TAggregateKey, TEventKey >, TAggregateKey, TEventKey>)null);
        }

        public Task SaveAsync<TAggregate>(TAggregate aggregate)
            where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
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

            if (!string.IsNullOrWhiteSpace(_memoryDumpFile))
            {
                SerializerHelper.SaveToFile(_memoryDumpFile, _eventStream);
            }

            return Task.CompletedTask;
        }
    }
}
