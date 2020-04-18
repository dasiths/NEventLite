using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Extensions.Microsoft.DependencyInjection;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Samples.ConsoleApp.Handlers;
using NEventLite.Storage;
using NEventLite.StorageProviders.EventStore;
using NEventLite.StorageProviders.EventStore.Core;
using NEventLite.StorageProviders.InMemory;

namespace NEventLite.Samples.ConsoleApp
{
    public static class DependencyInjection
    {
        private const int SnapshotFrequency = 2;
        private const int PageSize = 100;

        public static ServiceProvider GetContainer(bool useEventStore = false)
        {
            //This path is used to save in memory storage
            var strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";

            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);

            var services = new ServiceCollection();
            services.AddSingleton<IClock, DefaultSystemClock>();
            services.AddSingleton<IEventPublisher, MyEventPublisher>();

            if (useEventStore)
            {
                services.AddScoped<IEventStoreSettings, EventStoreSettings>(
                    (sp) => new EventStoreSettings(SnapshotFrequency, PageSize));
                services.AddScoped<IEventStoreStorageConnectionProvider, EventStoreStorageConnectionProvider>();
                services.AddScoped<IEventStoreStorageCore, EventStoreStorageCore>();
                services.AddScoped<IEventStorageProvider<Guid>, EventStoreEventStorageProvider>();
                services.AddScoped<ISnapshotStorageProvider<Guid>, EventStoreSnapshotStorageProvider>();
            }
            else
            {
                services.AddScoped<IEventStorageProvider<Guid>>(
                    provider => new InMemoryEventStorageProvider(inMemoryEventStorePath));
                services.AddScoped<ISnapshotStorageProvider<Guid>>(
                    provider => new InMemorySnapshotStorageProvider(SnapshotFrequency, inMemorySnapshotStorePath));
            }

            // Add the repository registration manually
            // services.AddScoped<IRepository<Schedule, Guid, Guid>, Repository<Schedule, ScheduleSnapshot>>();
            // services.AddScoped<ISession<Schedule>, Session<Schedule>>();
            // or if prefer to you use the more detailed interface
            // services.AddScoped<ISession<Schedule, Guid, Guid>, Session<Schedule>>();

            // or add the by scanning for all aggregate types
            //services.ScanAndRegisterAggregates();

            services.AddScoped<CreateScheduleHandler>();
            services.AddScoped<CreateTodoHandler>();
            services.AddScoped<UpdateTodoNameHandler>();
            services.AddScoped<CompleteTodoHandler>();

            return services.BuildServiceProvider();
        }
    }
}