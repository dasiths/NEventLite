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
