using System;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.Common.Domain.Events
{
    public class ScheduleCreatedEvent : Event<Guid, Schedule, Guid>
    {
        public string ScheduleName { get; set; }

        public ScheduleCreatedEvent(Guid scheduleId, string scheduleName) : base(Guid.NewGuid(), scheduleId)
        {
            this.ScheduleName = scheduleName;
        }
    }
}
