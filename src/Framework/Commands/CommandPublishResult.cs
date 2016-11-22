using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Commands
{
    public class CommandPublishResult:ICommandPublishResult
    {
        public bool IsSucess { get; }
        public string FailReason { get; }

        public CommandPublishResult(bool isSucess, string failReason)
        {
            this.IsSucess = isSucess;
            FailReason = failReason;
        }
    }
}
