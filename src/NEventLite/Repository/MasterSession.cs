using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        private class MockAggregate : AggregateRoot
        {
        }

        private class AggregateTypeContainer
        {
            public AggregateInformation AggregateInformation { get; }
            public Func<object, object> GetByIdMethod { get; }
            public Func<object, Task> RepositorySaveMethod { get; }
            public Dictionary<object, object> TrackedItems { get; } =
                new Dictionary<object, object>(); // key, value of id, aggregate

            public AggregateTypeContainer(AggregateInformation aggregateInformation, Func<object, object> byIdMethod, Func<object, Task> repositorySaveMethod)
            {
                AggregateInformation = aggregateInformation;
                GetByIdMethod = byIdMethod;
                RepositorySaveMethod = repositorySaveMethod;
            }
        }

        private readonly Dictionary<Type, AggregateTypeContainer> _aggregateTypeContainerCache = new Dictionary<Type, AggregateTypeContainer>();
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

                var typeContainer = _aggregateTypeContainerCache[aggregateType];
                if (typeContainer.AggregateInformation.AggregateKey != id.GetType())
                {
                    throw new ArgumentException($"Id is not of type {typeContainer.AggregateInformation.AggregateKey.Name}", nameof(id));
                }

                var itemExists = typeContainer.TrackedItems.TryGetValue(id, out var result);
                if (itemExists)
                {
                    return (TAggregate)result;
                }

                result = await (Task<TAggregate>)typeContainer.GetByIdMethod(id);
                typeContainer.TrackedItems.Add(id, result);

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
                var typeContainer = _aggregateTypeContainerCache[aggregateType];

                var itemExists = typeContainer.TrackedItems.TryGetValue(id, out _);
                if (itemExists)
                {
                    throw new ArgumentException("Item with the same id is already tracked", nameof(aggregate));
                }

                typeContainer.TrackedItems.Add(id, aggregate);
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
                var typeContainer = _aggregateTypeContainerCache[aggregateType];

                var itemExists = typeContainer.TrackedItems.TryGetValue(id, out var result);
                if (itemExists)
                {
                    typeContainer.TrackedItems.Remove(id);
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
                foreach (var t in _aggregateTypeContainerCache)
                {
                    var typeContainer = t.Value;
                    var saveMethod = typeContainer.RepositorySaveMethod;
                    foreach (var keyValuePair in typeContainer.TrackedItems)
                    {
                        await saveMethod(keyValuePair.Value);
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
                foreach (var t in _aggregateTypeContainerCache)
                {
                    t.Value.TrackedItems.Clear();
                }
            }
            finally
            {
                _syncLock.Release();
            }
        }

        private void SetupAggregateInfo(Type aggregateType)
        {
            if (_aggregateTypeContainerCache.ContainsKey(aggregateType))
            {
                return;
            }

            var result = aggregateType.GetAggregateInformation();
            if (!result.IsValidResult)
            {
                throw new ArgumentException("The type is not a valid AggregateType", nameof(aggregateType));
            }

            var repositoryType = typeof(IRepository<,,>).MakeGenericType(result.Aggregate, result.AggregateKey, result.EventKey);
            var repository = _factory(repositoryType);

            if (repository == null)
            {
                throw new ArgumentException($"No repository implementation for {repositoryType.Name} could be found", nameof(aggregateType));
            }

            repository = repositoryType.Cast(repository); // convert to type of interface

            Func<object, object> getByIdMethodAsync = (id) =>
            {
                var genericMethod =
                    repositoryType.GetMethod(nameof(IRepository<MockAggregate, Guid, Guid>.GetByIdAsync));
                return genericMethod.Invoke(repository, new[] { id });
            }; // returns Task<TAggregate>

            Func<object, Task> saveMethodAsync = (aggregate) =>
            {
                var genericMethod = repositoryType.GetMethod(nameof(IRepository<MockAggregate, Guid, Guid>.SaveAsync));
                return (Task)genericMethod.Invoke(repository, new[] { aggregate });
            }; // returns Task

            _aggregateTypeContainerCache.Add(result.Aggregate,
                new AggregateTypeContainer(result, getByIdMethodAsync, saveMethodAsync));
        }

        private static object GetId<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot
        {
            var id = typeof(TAggregate).GetProperty(nameof(MockAggregate.Id)).GetMethod.Invoke(aggregate, null);
            return id;
        }

        public void Dispose()
        {
            _aggregateTypeContainerCache.Clear();
            _syncLock?.Dispose();
        }
    }
}