using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Schedule.Events
{
    public class TodoCreatedEvent : Event<Schedule>
    {
        public Guid TodoId { get; set; }
        public string Text { get; set; }

        public TodoCreatedEvent(Guid aggregateId, long targetVersion, Guid todoId, string text) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
            Text = text;
        }
    }
}