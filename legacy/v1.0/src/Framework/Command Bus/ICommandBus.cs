using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Command;

namespace NEventLite.Command_Bus
{
    public interface ICommandBus
    {
        Task<ICommandPublishResult> ExecuteAsync<T>(T command) where T:ICommand;
    }
}
