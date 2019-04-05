using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Samples.Common.Domain.Schedule.Events;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;

namespace NEventLite.Samples.Common.Domain.Schedule
{
    public partial class Schedule : AggregateRoot, ISnapshottable<ScheduleSnapshot>
    {
        public IList<Todo> Todos { get; private set; }
        public string ScheduleName { get; private set; }

        public Schedule()
        {
        }

        public ScheduleSnapshot TakeSnapshot()
        {
            return new ScheduleSnapshot(Guid.NewGuid(), Id, CurrentVersion)
            {
                ScheduleName = ScheduleName,
                Todos = Todos.Select(t => new ScheduleSnapshot.TodoSnapshot()
                {
                    Id = t.Id,
                    Text = t.Text,
                    IsCompleted = t.IsCompleted
                }).ToList()
            };
        }

        public Schedule(string scheduleName)
        {
            var newScheduleId = Guid.NewGuid();
            var @event = new ScheduleCreatedEvent(newScheduleId, scheduleName);
            ApplyEvent(@event);
        }

        public void AddTodo(string text)
        {
            var newTodoId = Guid.NewGuid();
            var @event = new TodoCreatedEvent(Id, CurrentVersion, newTodoId, text);
            ApplyEvent(@event);
        }

        public void UpdateTodo(Guid todoId, string text)
        {
            var @event = new TodoUpdatedEvent(Id, CurrentVersion, todoId, text);
            ApplyEvent(@event);
        }

        // async example
        public async Task CompleteTodoAsync(Guid todoId)
        {
            var @event = new TodoCompletedEvent(Id, CurrentVersion, todoId);
            await ApplyEventAsync(@event);
        }
    }
}
