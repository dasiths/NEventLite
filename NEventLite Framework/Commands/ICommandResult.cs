using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Commands
{
    public interface ICommandResult
    {
        int AggregateVersion { get; }
        bool IsSucess { get; }
        string RejectReason { get; }
    }
}
