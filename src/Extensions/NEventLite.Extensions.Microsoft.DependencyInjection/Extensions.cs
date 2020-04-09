using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Repository;

namespace NEventLite.Extensions.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static void ScanAndRegisterAggregates(this ServiceCollection services)
        {
            services.ScanAndRegisterAggregates(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ScanAndRegisterAggregates(this ServiceCollection services, IList<Assembly> assemblies)
        {
            var allAggregates = assemblies
                .SelectMany(a =>
                    a.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract & typeof(IAggregateRoot).IsAssignableFrom(t)))
                .ToList();

            var aggregates = allAggregates.Select(a =>
            {
                var interfaces = a.GetInterfaces();
                var snapshottable = interfaces.FirstOrDefault(i =>
                    i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ISnapshottable<>) ||
                                        i.GetGenericTypeDefinition() == typeof(ISnapshottable<,,>)));

                return new KeyValuePair<Type, Type>(a, snapshottable?.GetGenericArguments()[0]);
            }).ToList();

            foreach (var kvp in aggregates)
            {
                var aggregateType = kvp.Key;
                var snapshotType = kvp.Value;

                services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateType),
                    snapshotType != null
                        ? typeof(Repository<,>).MakeGenericType(aggregateType, snapshotType)
                        : typeof(EventOnlyRepository<>).MakeGenericType(aggregateType));

                services.AddScoped(typeof(ISession<>).MakeGenericType(aggregateType),
                    typeof(Session<>).MakeGenericType(aggregateType));
            }
        }
    }
}
