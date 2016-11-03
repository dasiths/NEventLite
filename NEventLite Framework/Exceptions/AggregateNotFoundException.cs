using System;

namespace NEventLite.Exceptions
{
    public class AggregateNotFoundException: Exception
    {
        public AggregateNotFoundException(string msg) : base(msg)
        {
            
        }
    }
}
