using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Events
{
    public class TodoCompletedEvent : Event<Guid, Guid>
    {
        public Guid TodoId { get; set; }

        public TodoCompletedEvent(Guid aggregateId, int targetVersion, Guid todoId) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
        }
    }
}