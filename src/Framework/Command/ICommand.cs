using NEventLite.Message;
using System;

namespace NEventLite.Commands
{
    public interface ICommand: IMessage
    {
        Guid AggregateId { get; }
    }
}
