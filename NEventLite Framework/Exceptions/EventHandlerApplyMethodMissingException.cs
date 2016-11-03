using System;

namespace NEventLite.Exceptions
{
    public class EventHandlerApplyMethodMissingException: Exception
    {
        public EventHandlerApplyMethodMissingException(string msg) : base(msg)
        {
            
        }
    }
}
