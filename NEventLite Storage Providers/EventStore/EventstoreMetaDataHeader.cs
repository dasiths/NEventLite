using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite_Storage_Providers.EventStore
{
    public class EventstoreMetaDataHeader
    {
        public string CLRType { get; set; }
        public int CommitNumber { get; set; }
    }
}
