using System;

namespace NEventLite_Example.Event
{
    public class NoteTitleChangedEvent : NEventLite.Event.Event
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
