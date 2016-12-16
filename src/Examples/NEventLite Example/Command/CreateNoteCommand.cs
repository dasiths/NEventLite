using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;

namespace NEventLite_Example.Commands
{
    public class CreateNoteCommand:Command
    {
        public string Title { get; private set; }
        public string Desc { get; private set; }
        public string Cat { get; private set; }
        
        public CreateNoteCommand(Guid correlationId, Guid aggregateId, int targetVersion, string title, string desc, string cat) : base(correlationId, aggregateId , targetVersion)
        {
            this.Title = title;
            this.Desc = desc;
            this.Cat = cat;
        }

    }
}
