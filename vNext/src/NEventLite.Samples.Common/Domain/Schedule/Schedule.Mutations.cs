using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Samples.Common.Domain.Schedule.Events;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;

namespace NEventLite.Samples.Common.Domain.Schedule
{
    public partial class Schedule
    {
        // event applying
        [InternalEventHandler]
        public void OnScheduleCreated(ScheduleCreatedEvent @event)
        {
            ScheduleName = @event.ScheduleName;
            Todos = new List<Todo>();
        }

        [InternalEventHandler]
        public void OnTodoCreated(TodoCreatedEvent @event)
        {
            var todo = new Todo(@event.TodoId, @event.Text);
            Todos.Add(todo);
        }

        [InternalEventHandler]
        public void OnTodoUpdated(TodoUpdatedEvent @event)
        {
            var todo = Todos.Single(t => t.Id == @event.TodoId);
            todo.Text = @event.Text;
        }

        // async event handler
        [InternalEventHandler]
        public async Task OnTodoCompletedAsync(TodoCompletedEvent @event)
        {
            var todo = Todos.Single(t => t.Id == @event.TodoId);
            todo.IsCompleted = true;
            await Task.CompletedTask;
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

        public void ApplySnapshot(ScheduleSnapshot snapshot)
        {
            ScheduleName = snapshot.ScheduleName;
            Todos = snapshot.Todos.Select(t => new Todo(t.Id, t.Text)
            {
                Text = t.Text
            }).ToList();
        }
    }
}
