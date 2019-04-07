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
    public class Schedule : AggregateRoot, ISnapshottable<ScheduleSnapshot>
    {
        public IList<Todo> Todos { get; private set; }
        public string ScheduleName { get; private set; }

        public Schedule()
        {
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
