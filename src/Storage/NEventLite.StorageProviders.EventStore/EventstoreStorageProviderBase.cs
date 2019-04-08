using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.StorageProviders.EventStore
{
    public abstract class EventStoreStorageProviderBase<TAggregate, TAggregateKey> where TAggregate : AggregateRoot<TAggregateKey, Guid>
    {
        protected abstract string GetStreamNamePrefix();

        protected string AggregateIdToStreamName(Type t, string id)
        {
            //Ensure first character of type name is in lower case

            var prefix = GetStreamNamePrefix();
            return $"{char.ToLower(prefix[0])}{prefix.Substring(1)}{t.Name}{id}";
        }

        private JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.None
            };
        }

        protected IEvent<TAggregate, TAggregateKey, Guid> DeserializeEvent(ResolvedEvent returnedEvent)
        {
            var header = JsonConvert.DeserializeObject<EventStoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), GetSerializerSettings());

            var returnType = Type.GetType(header.ClrType);

            return
                (IEvent<TAggregate, TAggregateKey, Guid>)JsonConvert.DeserializeObject
                (Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType, GetSerializerSettings());
        }

        protected EventData SerializeEvent(IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid> @event, int commitNumber)
        {
            var header = new EventStoreMetaDataHeader()
            {
                ClrType = GetClrTypeName(@event),
                CommitNumber = commitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, GetSerializerSettings())),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, GetSerializerSettings())));
        }

        protected TSnapshot DeserializeSnapshotEvent<TSnapshot>(ResolvedEvent returnedEvent)
        {
            var header = JsonConvert.DeserializeObject<EventStoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), GetSerializerSettings());

            var returnType = Type.GetType(header.ClrType);

            return
                (TSnapshot)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType,
                        GetSerializerSettings());
        }

        protected EventData SerializeSnapshotEvent<TSnapshot>(TSnapshot @event, int commitNumber)
            where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            var header = new EventStoreMetaDataHeader()
            {
                ClrType = GetClrTypeName(@event),
                CommitNumber = commitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, GetSerializerSettings())),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, GetSerializerSettings())));
        }

        private string GetClrTypeName(object @event)
        {
            return @event.GetType() + "," + @event.GetType().Assembly.GetName().Name;
        }
    }
}
