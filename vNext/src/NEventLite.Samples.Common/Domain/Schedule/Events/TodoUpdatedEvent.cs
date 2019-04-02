using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Schedule.Events
{
    public class TodoUpdatedEvent : Event<Guid, Schedule, Guid>
    {
        public Guid TodoId { get; set; }
        public string Text { get; set; }

        public TodoUpdatedEvent(Guid aggregateId, int targetVersion, Guid todoId, string text) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
            Text = text;
        }
    }
}