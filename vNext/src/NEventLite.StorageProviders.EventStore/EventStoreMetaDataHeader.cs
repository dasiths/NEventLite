using System;
using System.Collections.Generic;
using System.Text;

namespace NEventLite.StorageProviders.EventStore
{
    public class EventStoreMetaDataHeader
    {
        public string ClrType { get; set; }
        public int CommitNumber { get; set; }
    }
}
