using NEventLite.Logger;
using ServiceStack.Redis;

namespace NEventLite_Example.Util
{
    public static class RedisConnection
    {
        private const string RedisConnectionString = "EventSourcingTest:redisPassword123@redis-12322.c10.us-east-1-3.ec2.cloud.redislabs.com:12322";

        private static IRedisClientsManager _manager;

        public static IRedisClientsManager GetClientManager()
        {
            if (_manager == null)
            {
                LogManager.Log("Connecting to Redis server...", LogSeverity.Information);
                _manager = new RedisManagerPool(RedisConnectionString);
            }

            return _manager;
        }

        public static void CloseClients()
        {
            if (_manager != null)
            {
                _manager.Dispose();
                _manager = null;

                LogManager.Log("Closing Connection to Redis server...", LogSeverity.Information);
            }
        }

    }
}
