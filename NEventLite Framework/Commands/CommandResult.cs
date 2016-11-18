using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Commands
{
    public class CommandResult:ICommandResult
    {
        public int AggregateVersion { get; }
        public bool IsSucess { get; }
        public string RejectReason { get; }

        public CommandResult(int aggregateVersion, bool isSucess, string rejectReason)
        {
            AggregateVersion = aggregateVersion;
            this.IsSucess = isSucess;
            RejectReason = rejectReason;
        }
    }
}
