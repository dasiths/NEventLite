using System;

namespace NEventLite_Example.Event
{
    public class NoteCategoryChangedEvent : NEventLite.Event.Event
    {
        private static int _currrentTypeVersion = 1;

        public string Cat { get; set; }

        public NoteCategoryChangedEvent(Guid aggregateId, int version, string cat)
            : base(aggregateId, version, _currrentTypeVersion)
        {
            this.Cat = cat;
        }
    }
}
