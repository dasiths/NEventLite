using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteTitleChangedEvent : Event
    {
        public string title { get; set; }

        public NoteTitleChangedEvent(Guid aggregateID, int version, string title):base(aggregateID, version)
        {
            this.title = title;
        }
    }
}
