using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Events
{
    public class TodoUpdatedEvent : Event<Guid, Guid>
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