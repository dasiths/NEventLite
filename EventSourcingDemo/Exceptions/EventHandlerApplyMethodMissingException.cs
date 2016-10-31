using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Exceptions
{
    public class EventHandlerApplyMethodMissingException: Exception
    {
        public EventHandlerApplyMethodMissingException(string msg) : base(msg)
        {
            
        }
    }
}
