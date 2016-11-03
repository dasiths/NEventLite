using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteCategoryChangedEvent : Event
    {
        public string cat { get; set; }

        public NoteCategoryChangedEvent(Guid aggregateID, int version, string cat)
        {
            Setup(aggregateID,version);

            this.cat = cat;
        }
    }
}
