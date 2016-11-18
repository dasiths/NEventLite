using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;

namespace NEventLite.Command_Bus
{
    public interface ICommandBus
    {
        Task<ICommandResult> ExecuteAsync<T>(T command) where T:ICommand;
    }
}
