using System;
using System.IO;
using System.Reflection;
using Autofac;
using NEventLite.Core;
using NEventLite.Extensions.Autofac;
using NEventLite.Repository;
using NEventLite.Samples.Common;
using NEventLite.Samples.Common.Domain.Schedule;
using NEventLite.Samples.Common.Domain.Schedule.Snapshots;
using NEventLite.Samples.Common.Handlers;
using NEventLite.Storage;
using NEventLite.StorageProviders.InMemory;
using Module = Autofac.Module;

namespace NEventLite.Samples.Autofac
{
    public class NEventLiteModule : Module
    {
        private const int SnapshotFrequency = 2;

        protected override void Load(ContainerBuilder builder)
        {
            //This path is used to save in memory storage
            var strTempDataFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";

            //create temp directory if it doesn't exist
            new FileInfo(strTempDataFolderPath).Directory?.Create();

            var inMemoryEventStorePath = $@"{strTempDataFolderPath}events.stream.dump";
            var inMemorySnapshotStorePath = $@"{strTempDataFolderPath}events.snapshot.dump";

            File.Delete(inMemoryEventStorePath);
            File.Delete(inMemorySnapshotStorePath);

            builder.RegisterType<DefaultSystemClock>().As<IClock>().InstancePerLifetimeScope();
            builder.RegisterType<MyEventPublisher>().As<IEventPublisher>().InstancePerLifetimeScope();

            builder.Register(c => new InMemoryEventStorageProvider(inMemoryEventStorePath))
                .As<IEventStorageProvider<Guid>>().InstancePerLifetimeScope();
            builder.Register(c => new InMemorySnapshotStorageProvider(SnapshotFrequency, inMemorySnapshotStorePath))
                .As<ISnapshotStorageProvider<Guid>>().InstancePerLifetimeScope();

            builder
                .ScanAndRegisterAggregates()
                .RegisterMasterSession();

            //Or add the repository registration manually
            //builder.RegisterType<Repository<Schedule, ScheduleSnapshot, Guid, Guid, Guid>>().As<IRepository<Schedule, Guid, Guid>>().InstancePerLifetimeScope();
            //builder.RegisterType<Session<Schedule>>().As<ISession<Schedule>>().InstancePerLifetimeScope();

            builder.RegisterType<CreateScheduleHandler>();
            builder.RegisterType<CreateTodoHandler>();
            builder.RegisterType<UpdateTodoNameHandler>();
            builder.RegisterType<CompleteTodoHandler>();
        }
    }
}