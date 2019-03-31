using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp.Domain.Events
{
    public class TodoCreatedEvent : Event<Guid, Guid>
    {
        public Guid TodoId { get; set; }
        public string Text { get; set; }

        public TodoCreatedEvent(Guid aggregateId, int targetVersion, Guid todoId, string text) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
            Text = text;
        }
    }
}