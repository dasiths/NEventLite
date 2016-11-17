using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Domain;
using NEventLite.Exceptions;

namespace NEventLite.Extensions
{
    public static class EventExtension
    {
        public static void InvokeOnAggregate(this Events.IEvent @event, AggregateRoot aggregate, string methodName)
        {
            var method = ReflectionHelper.GetMethod(aggregate.GetType(), methodName, new Type[] { @event.GetType() }); //Find the right method

            if (method != null)
            {
                method.Invoke(aggregate, new object[] { @event }); //invoke with the event as argument

                // or we can use dynamics
                //dynamic d = this;
                //dynamic e = @event;
                //d.Apply(e);
            }
            else
            {
                throw new EventHandlerApplyMethodMissingException($"No event Apply method found on {aggregate.GetType()} for {@event.GetType()}");
            }
        }
    }
}
