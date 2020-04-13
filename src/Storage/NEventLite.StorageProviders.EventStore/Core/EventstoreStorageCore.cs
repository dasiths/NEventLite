using System;
using System.Text;
using EventStore.ClientAPI;
using NEventLite.Core;
using NEventLite.Core.Domain;
using Newtonsoft.Json;

namespace NEventLite.StorageProviders.EventStore.Core
{
    public class EventStoreStorageCore : IEventStoreStorageCore
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None
        };

        public EventStoreStorageCore()
        {
        }

        private static string GetClrTypeName(object @event)
        {
            return @event.GetType() + "," + @event.GetType().Assembly.GetName().Name;
        }

        public string TypeToStreamName<T>(string id, string prefix)
        {
            //Ensure first character of type name is in lower case
            return $"{char.ToLower(prefix[0])}{prefix.Substring(1)}{typeof(T).Name}{id}";
        }
        
        public IEvent<TAggregate, TAggregateKey, Guid> DeserializeEvent<TAggregate, TAggregateKey>(ResolvedEvent returnedEvent)
            where TAggregate : AggregateRoot<TAggregateKey, Guid>
        {
            var header = JsonConvert.DeserializeObject<EventStoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), _serializerSettings);

            var returnType = Type.GetType(header.ClrType);

            return
                (IEvent<TAggregate, TAggregateKey, Guid>)JsonConvert.DeserializeObject
                (Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType, _serializerSettings);
        }

        public EventData SerializeEvent<TAggregateKey>(
            IEvent<AggregateRoot<TAggregateKey, Guid>, TAggregateKey, Guid> @event,
            long commitNumber)
        {
            var header = new EventStoreMetaDataHeader()
            {
                ClrType = GetClrTypeName(@event),
                CommitNumber = commitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, _serializerSettings)),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, _serializerSettings)));
        }

        public TSnapshot DeserializeSnapshotEvent<TSnapshot>(ResolvedEvent returnedEvent)
        {
            var header = JsonConvert.DeserializeObject<EventStoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), _serializerSettings);

            var returnType = Type.GetType(header.ClrType);

            return
                (TSnapshot)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType,
                    _serializerSettings);
        }

        public EventData SerializeSnapshotEvent<TSnapshot, TAggregateKey>(TSnapshot @event, long commitNumber)
            where TSnapshot : ISnapshot<TAggregateKey, Guid>
        {
            var header = new EventStoreMetaDataHeader()
            {
                ClrType = GetClrTypeName(@event),
                CommitNumber = commitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, _serializerSettings)),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, _serializerSettings)));
        }
    }
}
