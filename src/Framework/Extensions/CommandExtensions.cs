using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;
using NEventLite.Exceptions;

namespace NEventLite.Extensions
{
    public static class CommandExtensions
    {
        public static ICommandResult EnsureSuccess(this ICommandResult commandResult)
        {
            if (commandResult.IsSucess == false)
            {
                throw new CommandExecutionFailedException($"Command failed with message: {commandResult.RejectReason}");
            }

            return commandResult;
        }
    }
}
