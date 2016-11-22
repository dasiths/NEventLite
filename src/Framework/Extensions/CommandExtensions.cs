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
        public static void EnsurePublished(this ICommandPublishResult commandPublishResult)
        {
            if (commandPublishResult.IsSucess == false)
            {
                throw new CommandExecutionFailedException($"Command failed with message: {commandPublishResult.FailReason}");
            }
        }
    }
}
