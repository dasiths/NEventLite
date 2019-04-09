using System;
using System.Threading.Tasks;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;

namespace NEventLite.Samples.ConsoleApp.Handlers
{
    public class CreateScheduleHandler
    {
        private readonly ISession<Schedule> _session;

        public CreateScheduleHandler(ISession<Schedule> session)
        {
            _session = session;
        }

        public async Task<Guid> HandleAsync(string scheduleName)
        {
            var schedule = new Schedule(scheduleName);
            _session.Attach(schedule);
            await _session.SaveAsync();
            return schedule.Id;
        }
    }
}
