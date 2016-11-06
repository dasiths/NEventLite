using System;

namespace NEventLite.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}
