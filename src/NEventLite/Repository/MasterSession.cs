using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NEventLite.Core.Domain;
using NEventLite.Util;

namespace NEventLite.Repository
{
    public class MasterSession : IMasterSession
    {
        public delegate object ServiceFactory(Type type); 

        private sealed class MockAggregate : AggregateRoot
        {
        }

        private readonly Dictionary<Type, Func<object, object>> _repositoryGetByIdMethods = new Dictionary<Type, Func<object, object>>();
        private readonly Dictionary<Type, Func<object, Task>> _repositorySaveMethods = new Dictionary<Type, Func<object, Task>>();
        private readonly Dictionary<Type, AggregateInformation> _aggregateInformationCache = new Dictionary<Type, AggregateInformation>();
        private readonly Dictionary<Type, Dictionary<object, object>> _allTrackedItems = new Dictionary<Type, Dictionary<object, object>>(); // type, key, value
        private readonly ServiceFactory _factory;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        public MasterSession(ServiceFactory factory)
        {
            _factory = factory;
        }

        public async Task<TAggregate> GetByIdAsync<TAggregate>(object id) where TAggregate : IAggregateRoot
        {
            await _syncLock.WaitAsync();

            try
            {
                var aggregateType = typeof(TAggregate);
                SetupAggregateInfo(aggregateType);

                var aggregateInfo = _aggregateInformationCache[aggregateType];
                if (aggregateInfo.AggregateKey != id.GetType())
                {
                    throw new ArgumentException($"Id is not of type {aggregateInfo.AggregateKey.Name}", nameof(id));
                }

                var itemExists = _allTrackedItems[aggregateType].TryGetValue(id, out var result);
                if (itemExists)
                {
                    return (TAggregate)result;
                }

                result = await (Task<TAggregate>)_repositoryGetByIdMethods[aggregateType](id);
                _allTrackedItems[aggregateType].Add(id, result);

                return (TAggregate)result;
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public void Attach<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot
        {
            _syncLock.Wait();

            try
            {
                var aggregateType = typeof(TAggregate);
                SetupAggregateInfo(aggregateType);

                var id = GetId(aggregate);

                var itemExists = _allTrackedItems[aggregateType].TryGetValue(id, out var result);
                if (itemExists)
                {
                    throw new ArgumentException("Item with the same id is already tracked", nameof(aggregate));
                }

                _allTrackedItems[aggregateType].Add(id, aggregate);
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public void Detach<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot
        {
            _syncLock.Wait();

            try
            {
                var aggregateType = typeof(TAggregate);
                SetupAggregateInfo(aggregateType);

                var id = GetId(aggregate);

                var itemExists = _allTrackedItems[aggregateType].TryGetValue(id, out var result);
                if (itemExists)
                {
                    _allTrackedItems[aggregateType].Remove(id);
                }
                else
                {
                    throw new ArgumentException("Item with the same id is not tracked", nameof(aggregate));
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public async Task SaveAsync()
        {
            await _syncLock.WaitAsync();

            try
            {
                foreach (var t in _allTrackedItems)
                {
                    var saveMethod = _repositorySaveMethods[t.Key];
                    foreach (var keyValuePair in t.Value)
                    {
                        await (Task)saveMethod(keyValuePair.Value);
                    }
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public void DetachAll()
        {
            _syncLock.Wait();

            try
            {
                foreach (var t in _allTrackedItems)
                {
                    t.Value.Clear();
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }

        private void SetupAggregateInfo(Type aggregateType)
        {
            if (_aggregateInformationCache.ContainsKey(aggregateType))
            {
                return;
            }

            var result = aggregateType.GetAggregateInformation();
            if (!result.IsValidResult)
            {
                throw new ArgumentException("The type is not a valid AggregateType", nameof(aggregateType));
            }

            var repositoryType =
                typeof(IRepository<,,>).MakeGenericType(result.Aggregate, result.AggregateKey, result.EventKey);
            var repository = _factory(repositoryType);

            if (repository == null)
            {
                throw new ArgumentException($"No repository implementation for {repositoryType.Name} could be found", nameof(aggregateType));
            }

            repository = repositoryType.Cast(repository); // convert to type of interface

            _aggregateInformationCache.Add(aggregateType, result);
            _repositoryGetByIdMethods.Add(aggregateType, id =>
                {
                    var genericMethod = repositoryType.GetMethod(nameof(IRepository<MockAggregate, Guid, Guid>.GetByIdAsync));
                    return genericMethod.Invoke(repository, new[] { id });
                });

            _repositorySaveMethods.Add(aggregateType, aggregate =>
            {
                var genericMethod = repositoryType.GetMethod(nameof(IRepository<MockAggregate, Guid, Guid>.SaveAsync));
                return (Task)genericMethod.Invoke(repository, new[] { aggregate });
            });

            _allTrackedItems.Add(aggregateType, new Dictionary<object, object>());
        }

        private static object GetId<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot
        {
            var id = typeof(TAggregate).GetProperty(nameof(MockAggregate.Id)).GetMethod.Invoke(aggregate, null);
            return id;
        }
    }
}