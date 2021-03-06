﻿using System;
using System.Threading.Tasks;
using NEventLite.Repository;
using NEventLite.Samples.Common.Domain.Schedule;

namespace NEventLite.Samples.Common.Handlers
{
    public class CompleteTodoHandler
    {
        private readonly IMasterSession _session; // This examples use the IMasterSession

        public CompleteTodoHandler(IMasterSession session)
        {
            _session = session;
        }

        public async Task HandleAsync(Guid scheduleId, Guid todoId)
        {
            var schedule = await _session.GetByIdAsync<Schedule>(scheduleId);
            await schedule.CompleteTodoAsync(todoId);
            await _session.SaveAsync();
        }
    }
}