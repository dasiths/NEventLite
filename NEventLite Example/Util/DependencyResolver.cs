using Autofac;
using NEventLite.Events;
using NEventLite.Event_Bus;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite_Example.Domain;
using NEventLite_Example.Event_Bus;
using NEventLite_Example.Repository;
using NEventLite_Example.Storage;
using NEventLite_Storage_Providers.InMemory;

namespace NEventLite_Example.Util
{
    public class DependencyResolver
    {
        private IContainer Container { get; }

        public DependencyResolver()
        {
            // Create your builder.
            var builder = new ContainerBuilder();

            //-------- Event Stores ------------

            //Event store connection settings are in EventstoreEventStorageProvider class
            //If you don't have eventstore installed comment our the line below
            //builder.RegisterType<MyEventstoreEventStorageProvider>().As<IEventStorageProvider>().SingleInstance();

            builder.RegisterType<InMemoryEventStorageProvider>().As<IEventStorageProvider>().PreserveExistingDefaults().SingleInstance();

            //----------------------------------

            //-------- Snapshot Stores ----------

            //Event store connection settings are in EventstoreConnection class
            //If you don't have eventstore installed comment out the line below
            //builder.RegisterType<MyEventstoreSnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();

            //Redis connection settings are in RedisConnection class
            //builder.RegisterType<MyRedisSnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();

            builder.RegisterType<InMemorySnapshotStorageProvider>().As<ISnapshotStorageProvider>().PreserveExistingDefaults().SingleInstance();

            //----------------------------------

            //Event Bus
            builder.RegisterType<InMemoryEventBus>().As<IEventBus>().SingleInstance();

            //This will resolve and bind storage types to a concrete repositoryBase of <T> as needed
            builder.RegisterGeneric(typeof(RepositoryBase<>)).As(typeof(IRepositoryBase<>)).SingleInstance();

            Container = builder.Build();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
