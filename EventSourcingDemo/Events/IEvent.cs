using System;

namespace EventSourcingDemo.Events
{
    public interface IEvent
    {
        Guid Id { get; }
    }
}
