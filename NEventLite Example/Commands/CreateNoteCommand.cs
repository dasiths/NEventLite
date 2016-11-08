﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;

namespace NEventLite_Example.Commands
{
    public class CreateNoteCommand:Command
    {
        public string title { get; set; }
        public string desc { get; set; }
        public string cat { get; set; }
        
        public CreateNoteCommand(Guid id, int targetVersion, string title, string desc, string cat) : base(id, Guid.Empty, targetVersion)
        {
            this.title = title;
            this.desc = desc;
            this.cat = cat;
        }

    }
}