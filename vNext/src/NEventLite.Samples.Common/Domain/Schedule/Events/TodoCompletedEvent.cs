using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Schedule.Events
{
    public class TodoCompletedEvent : Event<Schedule, Guid, Guid>
    {
        public Guid TodoId { get; set; }

        public TodoCompletedEvent(Guid aggregateId, int targetVersion, Guid todoId) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
        }
    }
}