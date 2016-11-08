using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Logger;

namespace NEventLite_Example.Logging
{
    public class ConsoleLogger:ILogger
    {
        public void Log(string message, LogSeverity severity)
        {
            Console.WriteLine(message);
        }
    }
}
