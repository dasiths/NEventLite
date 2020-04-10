using System;
using EventStore.ClientAPI;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.StorageProviders.EventStore.Core
{
    public interface IEventStoreStorageCore
    {
        string TypeToStreamName<T>(string id, string prefix);
        IEvent<TAggregate, TAggregateKey, Guid> DeserializeEvent<TAggregate, TAggregateKey>(ResolvedEvent returnedEvent) where TAggregate : AggregateRoot<TAggregateKey, Guid>;
        EventData SerializeEvent<TAggregateKey>(IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid> @event, int commitNumber);
        TSnapshot DeserializeSnapshotEvent<TSnapshot>(ResolvedEvent returnedEvent);

        EventData SerializeSnapshotEvent<TSnapshot, TAggregateKey>(TSnapshot @event, int commitNumber)
            where TSnapshot : ISnapshot<TAggregateKey, Guid>;
    }
}