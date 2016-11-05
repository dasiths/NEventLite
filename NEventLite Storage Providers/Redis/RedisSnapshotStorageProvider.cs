using System;
using Newtonsoft.Json;
using NEventLite.Storage;
using ServiceStack.Redis;

namespace NEventLite_Storage_Providers.Redis
{
    public abstract class RedisSnapshotStorageProvider : ISnapshotStorageProvider
    {
        private BasicRedisClientManager clientsManager = null;

        public RedisSnapshotStorageProvider()
        {
            clientsManager = GetClientsManager();
        }

        public abstract BasicRedisClientManager GetClientsManager();

        public int SnapshotFrequency { get; set; }

        public NEventLite.Snapshot.Snapshot GetSnapshot(Type aggregateType, Guid aggregateId)
        {

            NEventLite.Snapshot.Snapshot snapshot = null;

            using (IRedisClient redis = clientsManager.GetClient())
            {
                var strSnapshot = redis.GetValue(aggregateId.ToString());

                if (strSnapshot != "")
                {
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    snapshot = JsonConvert.DeserializeObject<NEventLite.Snapshot.Snapshot>(
                    strSnapshot, serializerSettings);
                }
            }

            return snapshot;

        }

        public void SaveSnapshot(Type aggregateType, NEventLite.Snapshot.Snapshot snapshot)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {

                JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                var strSnapshot = JsonConvert.SerializeObject(snapshot, serializerSettings);

                redis.SetValue(snapshot.AggregateId.ToString(), strSnapshot);

            }
        }
    }
}
