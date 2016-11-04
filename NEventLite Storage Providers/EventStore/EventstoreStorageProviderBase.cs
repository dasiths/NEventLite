using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite_Storage_Providers.EventStore
{
    public abstract class EventstoreStorageProviderBase
    {
        protected abstract string GetStreamNamePrefix();

        protected string AggregateIdToStreamName(Type t, Guid id)
        {
            //Ensure first character of type name is in lower camel case

            var prefix = GetStreamNamePrefix();

            return $"{char.ToLower(prefix[0])}{prefix.Substring(1)}{t.Name}{id.ToString("N")}";
        }
    }
}
