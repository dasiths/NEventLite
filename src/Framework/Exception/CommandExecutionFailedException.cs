using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Exceptions
{
    public class CommandExecutionFailedException:Exception
    {
        public CommandExecutionFailedException(string msg) : base(msg)
        {
            
        }
    }
}
