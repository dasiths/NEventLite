using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Logger;
using NEventLite.Scope;

namespace NEventLite_Example.Util
{
    public class MyLifeTimeScope:LifeTimeScope
    {
        protected override void OnStart()
        {
            LogManager.Log("Scope Started...", LogSeverity.Warning);
        }

        protected override void OnEnd()
        {
            EventstoreConnection.CloseConnection();
            RedisConnection.CloseClients();
            LogManager.Log("Scope Ended...", LogSeverity.Warning);
        }
    }
}
