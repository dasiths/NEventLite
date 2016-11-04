using System;

namespace NEventLite.Exceptions
{
    public class AggregateCreationException: Exception
    {
        public AggregateCreationException(string msg) : base(msg)
        {
            
        }
    }
}
