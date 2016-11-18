using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Commands;
using NEventLite.Command_Bus;
using NEventLite.Command_Handlers;
using NEventLite_Example.Command_Handlers;

namespace NEventLite_Example.Command_Bus
{
    public class MyCommandBus:ICommandBus
    {
        private readonly NoteCommandHandler _noteCommandHandler;

        public MyCommandBus(NoteCommandHandler noteCommandHandler)
        {
            _noteCommandHandler = noteCommandHandler;
        }

        public async Task<ICommandResult> ExecuteAsync<T>(T command) where T:ICommand
        {
            var handler = _noteCommandHandler as ICommandHandler<T>;

            if (handler != null)
            {
                var result = await Task.Run(()=> handler.Handle(command));
                return result;
            }
            else
            {
                throw new NullReferenceException($"Command handler not found for {typeof(T)}");
            }
        }
    }
}
