using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using NEventLite.Repository;
using NEventLite.Util;

namespace NEventLite.Extensions.Autofac
{
    public static class Extensions
    {
        public static ContainerBuilder RegisterMasterSession(this ContainerBuilder services)
        {
            services.Register(c =>
                {
                    var context = c.Resolve<IComponentContext>();
                    return new MasterSession.ServiceFactory(context.Resolve);
                })
                .InstancePerLifetimeScope();
            services.RegisterType<MasterSession>().As<IMasterSession>()
                .InstancePerLifetimeScope();

            return services;
        }

        public static ContainerBuilder ScanAndRegisterAggregates(this ContainerBuilder services)
        {
            services.ScanAndRegisterAggregates(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static ContainerBuilder ScanAndRegisterAggregates(this ContainerBuilder services, IList<Assembly> assemblies)
        {
            foreach (var a in assemblies.GetAllAggregates())
            {
                services.RegisterAggregate(a);
            }

            return services;
        }

        public static ContainerBuilder RegisterAggregate(this ContainerBuilder services, AggregateInformation a)
        {
            // Register full generic types
            services.RegisterType(a.Snapshot != null
                    ? typeof(Repository<,,,,>).MakeGenericType(a.Aggregate, a.Snapshot, a.AggregateKey, a.EventKey, a.SnapshotKey)
                    : typeof(EventOnlyRepository<,,>).MakeGenericType(a.Aggregate, a.AggregateKey, a.EventKey))
                .As(typeof(IRepository<,,>).MakeGenericType(a.Aggregate, a.AggregateKey, a.EventKey))
                .InstancePerLifetimeScope();

            services.RegisterType(typeof(Session<,,>).MakeGenericType(a.Aggregate, a.AggregateKey, a.EventKey))
                .As(typeof(ISession<,,>).MakeGenericType(a.Aggregate, a.AggregateKey, a.EventKey))
                .InstancePerLifetimeScope();

            // Register the convenience GUID scoped ISession interface as well
            if (a.AggregateKey == typeof(Guid) && a.EventKey == typeof(Guid))
            {
                services.RegisterType(typeof(Session<>).MakeGenericType(a.Aggregate))
                    .As(typeof(ISession<>).MakeGenericType(a.Aggregate))
                    .InstancePerLifetimeScope();
            }

            return services;
        }
    }
}
