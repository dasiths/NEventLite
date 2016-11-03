using System;

namespace NEventLite.Exceptions
{
    public class ConcurrencyException: Exception
    {
        public ConcurrencyException(string msg) : base(msg)
        {
            
        }
    }
}
