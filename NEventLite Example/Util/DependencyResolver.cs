using Autofac;
using NEventLite.Repository;
using NEventLite.Storage;
using NEventLite_Example.Storage;
using NEventLite_Example.Unit_Of_Work;
using NEventLite_Storage_Providers.InMemory;

namespace NEventLite_Example.Util
{
    public class DependencyResolver
    {
        private static IContainer Container { get; set; }

        public DependencyResolver()
        {
            // Create your builder.
            var builder = new ContainerBuilder();

            //If you don't have eventstore installed use the InMemoryEventStorageProvider and comment out EventstoreEventStorageProvider line here.
            //Event store connection settings are in EventstoreEventStorageProvider class
            builder.RegisterType<MyEventstoreEventStorageProvider>().As<IEventStorageProvider>().SingleInstance();
            //builder.RegisterType<InMemoryEventStorageProvider>().As<IEventStorageProvider>().SingleInstance();

            //If you don't have eventstore installed use the InMemorySnapshotStorageProvider and comment out EventstoreEventStorageProvider line here.
            //Event store connection settings are in EventstoreConnection class
            builder.RegisterType<MyEventstoreSnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();
            //Redis connection settings are in RedisConnection class
            //builder.RegisterType<MyRedisSnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();
            //builder.RegisterType<InMemorySnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();

            builder.RegisterType<ChangeTrackingContext>().AsSelf().SingleInstance(); //InstancePerOwned(typeof(IRepository<>))
            builder.RegisterType<MyUnitOfWork>().AsSelf().SingleInstance(); //InstancePerOwned(typeof(IRepository<>))

            //This will resolve and bind storage types to a concrete repository of <T> as needed
            //builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).SingleInstance();

            Container = builder.Build();
        }

        public T ResolveDependecy<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
