using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Logger;

namespace NEventLite_Example.Logging
{
    public class ConsoleLogger : ILogger
    {

        private object _lockObject = new object();

        public void Log(string message, LogSeverity severity)
        {
            lock (_lockObject)
            {
                switch (severity)
                {
                    case LogSeverity.Debug:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        }
                    case LogSeverity.Information:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        }
                    case LogSeverity.Warning:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        }

                    case LogSeverity.Critical:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        }
                }

                Console.WriteLine("   > " + message);

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
