using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Events
{
    public class NoteCreatedEvent : Event
    {
        public string title { get; set; }
        public string desc { get; set; }
        public string cat { get; set; }
        public DateTime createdTime { get; set; }

        public NoteCreatedEvent(Guid aggregateID, int version, string title, string desc, string cat, DateTime createdTime)
        {
            Setup(aggregateID,version);

            this.title = title;
            this.desc = desc;
            this.cat = cat;
            this.createdTime = createdTime;
        }
    }
}
