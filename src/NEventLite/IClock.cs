using System;

namespace NEventLite
{
    public interface IClock
    {
        DateTimeOffset Now();
    }

    public class DefaultSystemClock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}
