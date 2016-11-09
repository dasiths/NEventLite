using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Logger;
using NEventLite.Scope;

namespace NEventLite_Example.Util
{
    public class MyLifeTimeScope:ILifeTimeScope
    {
        protected override void OnStart()
        {
            LogManager.Log("Scope Started...", LogSeverity.Information);
        }

        protected override void OnEnd()
        {
            LogManager.Log("Scope Ended...", LogSeverity.Information);
            EventstoreConnection.CloseConnection();
        }
    }
}
