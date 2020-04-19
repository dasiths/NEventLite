using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NEventLite.Core;
using NEventLite.Core.Domain;

namespace NEventLite.Util
{
    public static class AssemblyExtensions
    {
        public static IList<AggregateInformation> GetAllAggregates(this IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a =>
                    a.GetTypes()
                        .Select(t => t.GetAggregateInformation())
                        .Where(t => t.IsValidResult)).
                ToList();
        }

        public static AggregateInformation GetAggregateInformation(this Type aggregateType)
        {
            if (!aggregateType.IsClass || aggregateType.IsAbstract || 
                                      !typeof(IAggregateRoot).IsAssignableFrom(aggregateType))
            {
                return AggregateInformation.InvalidResult;
            }

            var genericParams = aggregateType.ExtractAggregateGenericParameters();

            if (genericParams.Count != typeof(AggregateRoot<,>).GetGenericArguments().Length)
            {
                return AggregateInformation.InvalidResult;
            }

            var aggregateKeyType = genericParams[0];
            var eventKeyType = genericParams[1];

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

            return AggregateInformation.ValidResult(aggregateType, aggregateKeyType, eventKeyType, snapshotType, snapshotKeyType);
        }

        internal static IList<Type> ExtractAggregateGenericParameters(this Type type)
        {
            if (typeof(IAggregateRoot).IsAssignableFrom(type))
            {
                if (type.BaseType != null)
                {
                    if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(AggregateRoot<,>))
                    {
                        return type.BaseType.GetGenericArguments();
                    }

                    return type.BaseType.ExtractAggregateGenericParameters();
                }
            }

            return new List<Type>();
        }
    }
}
