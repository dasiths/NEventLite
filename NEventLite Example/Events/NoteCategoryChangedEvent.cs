using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteCategoryChangedEvent : Event
    {
        private static int _currrentTypeVersion = 1;

        public string cat { get; set; }

        public NoteCategoryChangedEvent(Guid aggregateID, int version, string cat)
            : base(aggregateID, version, _currrentTypeVersion)
        {
            this.cat = cat;
        }
    }
}
