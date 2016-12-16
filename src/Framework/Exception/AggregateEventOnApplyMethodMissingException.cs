using System;

namespace NEventLite.Exceptions
{
    public class AggregateEventOnApplyMethodMissingException: Exception
    {
        public AggregateEventOnApplyMethodMissingException(string msg) : base(msg)
        {
            
        }
    }
}
