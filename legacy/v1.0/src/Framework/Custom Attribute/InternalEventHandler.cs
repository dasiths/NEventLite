using System;

namespace NEventLite.Custom_Attribute
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class InternalEventHandler : Attribute
    {
    }
}
