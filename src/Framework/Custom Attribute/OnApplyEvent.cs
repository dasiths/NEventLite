using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Custom_Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class OnApplyEvent : Attribute
    {
    }
}
