using System;
using System.Threading.Tasks;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;

namespace NEventLite.Samples.ConsoleApp.Handlers
{
    public class CreateTodoHandler
    {
        private readonly ISession<Schedule> _session;

        public CreateTodoHandler(ISession<Schedule> session)
        {
            _session = session;
        }

        public async Task<Guid> HandleAsync(Guid scheduleId, string todoName)
        {
            var schedule = await _session.GetByIdAsync(scheduleId);
            var id = schedule.AddTodo(todoName);
            await _session.SaveAsync();
            return id;
        }
    }
}