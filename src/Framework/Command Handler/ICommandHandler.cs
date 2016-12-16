using System.Threading.Tasks;
using NEventLite.Command;

namespace NEventLite.Command_Handler
{
    public interface ICommandHandler<T> where T:ICommand
    {
        Task HandleCommandAsync(T command);
    }
}
