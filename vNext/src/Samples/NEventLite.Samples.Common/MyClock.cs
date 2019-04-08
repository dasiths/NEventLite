using System;

namespace NEventLite.Samples.Common
{
    public class MyClock: IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }
    }
}
