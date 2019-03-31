using System;

namespace NEventLite.Exceptions
{
    public class AggregateEventOnApplyMethodMissingException : Exception
    {
        public AggregateEventOnApplyMethodMissingException(string message): base (message)
        {
        }
    }
}