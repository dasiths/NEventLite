using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;

namespace NEventLite_Example.Events
{
    public class NoteDescriptionChangedEvent:Event
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
