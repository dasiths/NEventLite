using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteTitleChangedEvent : Event
    {
        private static int _currrentTypeVersion = 1;

        public string title { get; set; }

        public NoteTitleChangedEvent(Guid aggregateID, int version, string title)
            : base(aggregateID, version, _currrentTypeVersion)
        {
            this.title = title;
        }
    }
}
