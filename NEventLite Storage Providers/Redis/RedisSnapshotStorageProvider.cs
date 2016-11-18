using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NEventLite.Storage;
using ServiceStack.Redis;

namespace NEventLite_Storage_Providers.Redis
{
    public abstract class RedisSnapshotStorageProvider : ISnapshotStorageProvider
    {
        private IRedisClientsManager _clientsManager = null;

        protected RedisSnapshotStorageProvider(int frequency)
        {
            _clientsManager = GetClientsManager();
            SnapshotFrequency = frequency;
        }

        public abstract IRedisClientsManager GetClientsManager();

        public int SnapshotFrequency { get; }

        public async Task<NEventLite.Snapshot.Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId)
        {

            NEventLite.Snapshot.Snapshot snapshot = null;

            using (IRedisClient redis = _clientsManager.GetClient())
            {
                var strSnapshot = redis.GetValue(aggregateId.ToString());

                if (string.IsNullOrEmpty(strSnapshot)==false)
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

        public async Task SaveSnapshotAsync(Type aggregateType, NEventLite.Snapshot.Snapshot snapshot)
        {
            using (IRedisClient redis = _clientsManager.GetClient())
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
