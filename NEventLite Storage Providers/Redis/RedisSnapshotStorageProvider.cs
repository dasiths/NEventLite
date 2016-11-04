using System;
using Newtonsoft.Json;
using NEventLite.Domain;
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

        public NEventLite.Snapshot.Snapshot GetSnapshot<T>(Guid aggregateId) where T : AggregateRoot
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

        public void SaveSnapshot<T>(NEventLite.Snapshot.Snapshot snapshot) where T : AggregateRoot
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
