using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;

namespace NEventLite_Example.Commands
{
    public class EditNoteCommand:Command
    {
        public string title { get; private set; }
        public string cat { get; private set; }

        public EditNoteCommand(Guid id, Guid aggregateId, int targetVersion, string title, string cat) : base(id, aggregateId, targetVersion)
        {
            this.title = title;
            this.cat = cat;
        }
    }
}
