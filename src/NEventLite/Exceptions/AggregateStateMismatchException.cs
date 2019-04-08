using System;

namespace NEventLite.Exceptions
{
    public class AggregateStateMismatchException: Exception
    {
        public AggregateStateMismatchException(string message): base(message)
        {
        }
    }
}
