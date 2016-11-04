using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NEventLite.Domain;
using NEventLite.Events;
using NEventLite.Snapshot;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreStorageProviderBase
    {
        private static JsonSerializerSettings _serializerSetting;
        protected abstract string GetStreamNamePrefix();
        protected string AggregateIdToStreamName(Type t, Guid id)
        {
            //Ensure first character of type name is in lower camel case

            var prefix = GetStreamNamePrefix();

            return $"{char.ToLower(prefix[0])}{prefix.Substring(1)}{t.Name}{id.ToString("N")}";
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            if (_serializerSetting == null)
            {
                _serializerSetting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
            }

            return _serializerSetting;
        }

        protected static Event DeserializeEvent(ResolvedEvent returnedEvent)
        {

            var header = JsonConvert.DeserializeObject<EventstoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), GetSerializerSettings());

            var returnType = Type.GetType(header.CLRType);

            return
                (Event)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType,
                        GetSerializerSettings());
        }
        protected static EventData SerializeEvent(Event @event, int CommitNumber)
        {
            var header = new EventstoreMetaDataHeader()
            {
                CLRType = GetClrTypeName(@event),
                CommitNumber = CommitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, GetSerializerSettings())),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, GetSerializerSettings())));
        }
        protected static Snapshot DeserializeSnapshotEvent(ResolvedEvent returnedEvent)
        {

            var header = JsonConvert.DeserializeObject<EventstoreMetaDataHeader>(
                Encoding.UTF8.GetString(returnedEvent.Event.Metadata), GetSerializerSettings());

            var returnType = Type.GetType(header.CLRType);

            return
                (Snapshot)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(returnedEvent.Event.Data), returnType,
                        GetSerializerSettings());
        }
        protected static EventData SerializeSnapshotEvent(Snapshot @event, int commitNumber)
        {
            var header = new EventstoreMetaDataHeader()
            {
                CLRType = GetClrTypeName(@event),
                CommitNumber = commitNumber
            };

            return new EventData(@event.Id, @event.GetType().Name, true,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, GetSerializerSettings())),
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, GetSerializerSettings())));
        }

        private static string GetClrTypeName(object @event)
        {
            return @event.GetType().ToString() + "," + @event.GetType().Assembly.GetName().Name;
        }
    }
}
