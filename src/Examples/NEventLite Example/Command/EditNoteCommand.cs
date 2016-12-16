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
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Cat { get; private set; }

        public EditNoteCommand(Guid correlationId, Guid aggregateId, int targetVersion, string title, string description, string cat) : base(correlationId, aggregateId, targetVersion)
        {
            this.Title = title;
            this.Description = description;
            this.Cat = cat;
        }
    }
}
