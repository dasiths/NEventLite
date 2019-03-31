﻿using System;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Samples.ConsoleApp.Domain.Events
{
    public class ScheduleCreatedEvent : Event<Guid, Guid>
    {
        public string ScheduleName { get; set; }

        public ScheduleCreatedEvent(Guid scheduleId, string scheduleName) : base(Guid.NewGuid(), scheduleId)
        {
            this.ScheduleName = scheduleName;
        }
    }
}
