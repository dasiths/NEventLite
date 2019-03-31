using System;

namespace NEventLite.Samples.ConsoleApp.Infrastructure
{
    public class MyClock: IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}
