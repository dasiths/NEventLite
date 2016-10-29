using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Events
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
