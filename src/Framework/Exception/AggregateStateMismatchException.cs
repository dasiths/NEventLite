using System;

namespace NEventLite.Exceptions
{
    public class AggregateStateMismatchException: Exception
    {
        public AggregateStateMismatchException(string msg) : base(msg)
        {
            
        }
    }
}
