using System;

namespace NEventLite_Example.Event
{
    public class NoteCreatedEvent : NEventLite.Event.Event
    {
        private static int _currrentTypeVersion = 1;

        public string Title { get; set; }
        public string Desc { get; set; }
        public string Cat { get; set; }
        public DateTime CreatedTime { get; set; }

        public NoteCreatedEvent(Guid aggregateId, int version, string title, string desc, string cat, DateTime createdTime)
            : base(aggregateId, version, _currrentTypeVersion)
        {
            this.Title = title;
            this.Desc = desc;
            this.Cat = cat;
            this.CreatedTime = createdTime;
        }
    }
}
