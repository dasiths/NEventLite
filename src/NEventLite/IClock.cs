using System;

namespace NEventLite
{
    public interface IClock
    {
        DateTimeOffset Now();
    }
}
