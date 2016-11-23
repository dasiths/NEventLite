using System;

namespace NEventLite.Commands
{
    public interface ICommand
    {
        Guid CorrelationId { get; }
        Guid AggregateId { get; }
    }
}
