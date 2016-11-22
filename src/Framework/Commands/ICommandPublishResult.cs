using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Commands
{
    public interface ICommandPublishResult
    {
        bool IsSucess { get; }
        string FailReason { get; }
    }
}
