using System;
using NEventLite.Message;

namespace NEventLite.Command
{
    public interface ICommand: IMessage
    {
        Guid AggregateId { get; }
    }
}
