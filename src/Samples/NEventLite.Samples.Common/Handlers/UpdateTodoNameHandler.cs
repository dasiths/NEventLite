using System;
using System.Threading.Tasks;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;

namespace NEventLite.Samples.Common.Handlers
{
    public class UpdateTodoNameHandler
    {
        private readonly ISession<Schedule> _session;

        public UpdateTodoNameHandler(ISession<Schedule> session)
        {
            _session = session;
        }

        public async Task HandleAsync(Guid scheduleId, Guid todoId, string todoName)
        {
            var schedule = await _session.GetByIdAsync(scheduleId);
            schedule.UpdateTodo(todoId, todoName);
            await _session.SaveAsync();
        }
    }
}