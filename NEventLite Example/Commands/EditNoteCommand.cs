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
        public string title { get; set; }
        public string cat { get; set; }

        public EditNoteCommand(Guid id, int targetVersion, string title, string cat) : base(id, targetVersion)
        {
            this.title = title;
            this.cat = cat;
        }
    }
}
