using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteTitleChangedEvent : Event
    {
        private static int _currrentTypeVersion = 1;

        public string Title { get; set; }

        public NoteTitleChangedEvent(Guid aggregateId, int version, string title)
            : base(aggregateId, version, _currrentTypeVersion)
        {
            this.Title = title;
        }
    }
}
