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
        public string title { get; private set; }
        public string desc { get; private set; }
        public string cat { get; private set; }
        
        public CreateNoteCommand(Guid id, Guid aggregateID, int targetVersion, string title, string desc, string cat) : base(id, aggregateID , targetVersion)
        {
            this.title = title;
            this.desc = desc;
            this.cat = cat;
        }

    }
}
