using System;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteCreatedEvent : Event
    {
        private static int _currrentTypeVersion = 1;

        public string title { get; set; }
        public string desc { get; set; }
        public string cat { get; set; }
        public DateTime createdTime { get; set; }

        public NoteCreatedEvent(Guid aggregateID, int version, string title, string desc, string cat, DateTime createdTime)
            : base(aggregateID, version, _currrentTypeVersion)
        {
            this.title = title;
            this.desc = desc;
            this.cat = cat;
            this.createdTime = createdTime;
        }
    }
}
