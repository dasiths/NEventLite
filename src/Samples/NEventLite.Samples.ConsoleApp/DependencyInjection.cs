using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NEventLite.Core;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Samples.ConsoleApp.Handlers;
using NEventLite.Storage;
using NEventLite.StorageProviders.EventStore;
using NEventLite.StorageProviders.InMemory;

namespace NEventLite.Samples.ConsoleApp
{
    public static class DependencyInjection
    {
        public static ServiceProvider GetContainer(bool useEventStore = false)
        {
            //This path is used to save in memory storage
            string strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";

            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);

            var services = new ServiceCollection();
            services.AddSingleton<IClock, MyClock>();
            services.AddSingleton<IEventPublisher<Schedule>, EventPublisher<Schedule>>();

            if (useEventStore)
            {
                services.AddScoped<IEventStoreStorageConnectionProvider, EventStoreStorageConnectionProvider>();
                services.AddScoped<IEventStorageProvider<Schedule>, EventStoreEventStorageProvider<Schedule>>();
                services.AddScoped<ISnapshotStorageProvider<ScheduleSnapshot>, EventStoreSnapshotStorageProvider<Schedule, ScheduleSnapshot>>();
            }
            else
            {
                services.AddScoped<IEventStorageProvider<Schedule>>(
                    provider => new InMemoryEventStorageProvider<Schedule>(inMemoryEventStorePath));
                services.AddScoped<ISnapshotStorageProvider<ScheduleSnapshot>>(
                    provider => new InMemorySnapshotStorageProvider<ScheduleSnapshot>(2, inMemorySnapshotStorePath));
            }

            services.AddScoped<IRepository<Schedule>, Repository<Schedule, ScheduleSnapshot>>();
            services.AddScoped<ISession<Schedule>, Session<Schedule>>();

            services.AddScoped<CreateScheduleHandler>();
            services.AddScoped<CreateTodoHandler>();
            services.AddScoped<UpdateTodoNameHandler>();
            services.AddScoped<CompleteTodoHandler>();

            return services.BuildServiceProvider();
        }
    }
}