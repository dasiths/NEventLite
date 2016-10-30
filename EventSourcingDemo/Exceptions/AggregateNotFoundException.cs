using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Exceptions
{
    public class AggregateNotFoundException: Exception
    {
        public AggregateNotFoundException(string msg) : base(msg)
        {
            
        }
    }
}
