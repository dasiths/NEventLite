using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Exceptions;

namespace NEventLite.Commands
{
    public class CommandPublishResult:ICommandPublishResult
    {
        public bool IsSucess { get; }
        public string FailReason { get; }
        public Exception ResultException { get; }

        public CommandPublishResult(bool isSucess, string failReason, Exception ex)
        {
            this.IsSucess = isSucess;
            FailReason = failReason;
            ResultException = ex;
        }

        public void EnsurePublished()
        {
            if (this.IsSucess == false)
            {
                throw new CommandExecutionFailedException(
                    $"Command failed with message: {this.FailReason} \n\n {this.ResultException?.Message}");
            }
        }
    }
}
