using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Util;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace EventSourcingDemo.Storage
{
    public class RedisSnapshotStorageProvider : ISnapshotStorageProvider
    {
        private BasicRedisClientManager clientsManager = null;

        public RedisSnapshotStorageProvider()
        {
            clientsManager = new BasicRedisClientManager(RedisConnection.redisConnectionString);
        }

        public Snapshot.Snapshot GetSnapshot(Guid aggregateId)
        {

            Snapshot.Snapshot snapshot = null;

            using (IRedisClient redis = clientsManager.GetClient())
            {
                var strSnapshot = redis.GetValue(aggregateId.ToString());

                if (strSnapshot != "")
                {
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    snapshot = JsonConvert.DeserializeObject<Snapshot.Snapshot>(
                    strSnapshot, serializerSettings);
                }
            }

            return snapshot;

        }

        public void SaveSnapshot(Snapshot.Snapshot snapshot)
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
