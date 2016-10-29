using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Events
{
    public class NoteTitleChangedEvent : Event
    {
        public string title { get; set; }

        public NoteTitleChangedEvent(Guid aggregateID, int version, string title)
        {
            Setup(aggregateID,version);

            this.title = title;
        }
    }
}
