using System;
using System.Threading.Tasks;
using NEventLite.Core;
using NEventLite.Core.Domain;
using NEventLite.Exceptions;

namespace NEventLite.Util
{
    public static class EventExtensions
    {
        public static async Task InvokeOnAggregateAsync<TAggregateKey, TEventKey>(this IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey> @event,
            AggregateRoot<TAggregateKey, TEventKey> aggregate, string methodName)
        {
            var method = ReflectionHelper.GetMethod(aggregate.GetType(), methodName, new Type[] { @event.GetType() }); //Find the right method

            if (method != null)
            {
                var task = method.Invoke(aggregate, new object[] { @event }); //invoke with the event as argument

                if (task != null && task.GetType() == typeof(Task))
                {
                    await (Task)task;
                }
            }
            else
            {
                throw new AggregateEventOnApplyMethodMissingException($"No event Apply method found on {aggregate.GetType()} for {@event.GetType()}");
            }
        }
    }
}
