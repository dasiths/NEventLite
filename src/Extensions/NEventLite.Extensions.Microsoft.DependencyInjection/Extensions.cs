using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Repository;
using NEventLite.Storage;

namespace NEventLite.Extensions.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static void ScanAndRegisterAggregates(this ServiceCollection services)
        {
            services.ScanAndRegisterAggregates<Guid, Guid>();
        }

        public static void ScanAndRegisterAggregates<TAggregateKey, TEventKey>(this ServiceCollection services)
        {
            services.ScanAndRegisterAggregates<TAggregateKey, TEventKey>(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ScanAndRegisterAggregates<TAggregateKey, TEventKey>(this ServiceCollection services, IList<Assembly> assemblies)
        {
            var allAggregates = assemblies
                .SelectMany(a =>
                    a.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract & typeof(AggregateRoot<TAggregateKey, TEventKey>).IsAssignableFrom(t)))
                .ToList();


            foreach (var aggregateType in allAggregates)
            {
                services.RegisterAggregate<TAggregateKey, TEventKey>(aggregateType);
            }
        }

        public static void RegisterAggregate<TAggregateKey, TEventKey>(this ServiceCollection services, Type aggregateType)
        {
            var snapshottableSimple = aggregateType.GetInterfaces().FirstOrDefault(i =>
                i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ISnapshottable<>)));

            var snapshottableComplex = aggregateType.GetInterfaces().FirstOrDefault(i =>
                i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ISnapshottable<,,>)));

            Type snapshotType = null;
            Type snapshotKeyType = null;

            if (snapshottableSimple != null)
            {
                snapshotType = snapshottableSimple.GetGenericArguments()[0];
                snapshotKeyType = typeof(Guid);
            }
            else if (snapshottableComplex != null)
            {
                snapshotType = snapshottableComplex.GetGenericArguments()[0];
                snapshotKeyType = snapshottableComplex.GetGenericArguments()[2];
            }

            // Register full generic types
            services.AddScoped(typeof(IRepository<,,>).MakeGenericType(aggregateType, typeof(TAggregateKey), typeof(TEventKey)),
                snapshotType != null
                    ? typeof(Repository<,,,,>).MakeGenericType(aggregateType, snapshotType, typeof(TAggregateKey),
                        typeof(TEventKey), snapshotKeyType)
                    : typeof(EventOnlyRepository<,,>).MakeGenericType(aggregateType, typeof(TAggregateKey), typeof(TEventKey)));

            services.AddScoped(typeof(ISession<,,>).MakeGenericType(aggregateType, typeof(TAggregateKey), typeof(TEventKey)),
                typeof(Session<,,>).MakeGenericType(aggregateType, typeof(TAggregateKey), typeof(TEventKey)));

            // Register the convenience GUID scoped ISession interface as well
            if (typeof(TAggregateKey) == typeof(Guid) && typeof(TEventKey) == typeof(Guid))
            {
                services.AddScoped(typeof(ISession<>).MakeGenericType(aggregateType),
                    typeof(Session<>).MakeGenericType(aggregateType));
            }
        }
    }
}
