using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EventSourcingDemo.Domain;
using EventSourcingDemo.Repository;
using EventSourcingDemo.Storage;

namespace EventSourcingDemo.Util
{
    public class DependencyResolver
    {
        private static IContainer Container { get; set; }

        public DependencyResolver()
        {
            // Create your builder.
            var builder = new ContainerBuilder();

            //If you don't have eventstore installed use the InMemoeryEventStorageProvider and comment out EventstoreEventStorageProvider line here.
            //Event store connection settings are in EventstoreEventStorageProvider class
            builder.RegisterType<EventstoreEventStorageProvider>().As<IEventStorageProvider>().SingleInstance();
            //builder.RegisterType<InMemoryEventStorageProvider>().As<IEventStorageProvider>().SingleInstance();

            //Can use Redis as cache/snapshot provider
            builder.RegisterType<InMemorySnapshotStorageProvider>().As<ISnapshotStorageProvider>().SingleInstance();

            //This will resolve and bind storage types to a concrete repository of <T> as needed
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).SingleInstance();
            Container = builder.Build();
        }

        public T ResolveDependecy<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
