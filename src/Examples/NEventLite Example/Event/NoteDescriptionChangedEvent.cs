using System;

namespace NEventLite_Example.Event
{
    public class NoteDescriptionChangedEvent:NEventLite.Event.Event
    {
        private static int _currrentTypeVersion = 1;

        public string Description { get; set; }

        public NoteDescriptionChangedEvent(Guid aggregateId, int version, string description)
            : base(aggregateId, version, _currrentTypeVersion)
        {
            this.Description = description;
        }
    }
}
