using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Util;
using ServiceStack.Redis;

namespace EventSourcingDemo.Storage
{
    class MyRedisSnapshotStorageProvider:NEventLite_Storage_Providers.Redis.RedisSnapshotStorageProvider
    {
        public override BasicRedisClientManager GetClientsManager()
        {
            return new BasicRedisClientManager(RedisConnection.redisConnectionString);
        }
    }
}
