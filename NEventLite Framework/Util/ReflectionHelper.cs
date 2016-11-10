using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Custom_Attributes;
using NEventLite.Domain;
using NEventLite.Events;

namespace NEventLite.Util
{
    public static class ReflectionHelper
    {
        private static Dictionary<Type, Dictionary<Type, string>> _AggregateEventHandlerCache =
            new Dictionary<Type, Dictionary<Type, string>>();

        public static Dictionary<Type, string> FindEventHandlerMethodsInAggregate(Type aggregateType)
        {

            if (_AggregateEventHandlerCache.ContainsKey(aggregateType) == false)
            {
                Type attributeType = typeof(EventHandlingMethod);
                var eventHandlers = new Dictionary<Type, string>();

                foreach (var methodInfo in aggregateType.GetMethods())
                {
                    var methodAttributes = methodInfo.GetCustomAttributes(attributeType, true);

                    foreach (var m in methodAttributes)
                    {
                        if (m.GetType() == attributeType)
                        {
                            var p = methodInfo.GetParameters();

                            if (p.Length == 1 && typeof(IEvent).IsAssignableFrom(p[0].ParameterType))
                            {
                                eventHandlers.Add(p[0].ParameterType, methodInfo.Name);
                                break;
                            }

                            break;
                        }

                    }
                }

                _AggregateEventHandlerCache.Add(aggregateType, eventHandlers);
            }


            return _AggregateEventHandlerCache[aggregateType];
        }
    }
}
