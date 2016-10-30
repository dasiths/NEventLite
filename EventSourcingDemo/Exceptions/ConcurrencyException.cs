using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo.Exceptions
{
    public class ConcurrencyException: Exception
    {
        public ConcurrencyException(string msg) : base(msg)
        {
            
        }
    }
}
