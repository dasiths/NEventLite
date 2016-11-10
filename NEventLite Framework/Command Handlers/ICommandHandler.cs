using NEventLite.Commands;

namespace NEventLite.Command_Handlers
{
    public interface ICommandHandler<T> where T:ICommand
    {
        ICommandResult Handle(T command);
    }
}
