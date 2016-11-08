using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Logger
{
    public static class LogManager
    {
        private static List<ILogger> _loggers = new List<ILogger>();

        public static void AddLogger(ILogger logger)
        {
            if (_loggers.Contains(logger) == false)
            {
                logger.Log($"Logging started at: {DateTime.Now}", LogSeverity.Information);
            }
        }

        public static void RemoveLogger(ILogger logger)
        {
            if (_loggers.Contains(logger) == true)
            {
                logger.Log($"Logging stopped at: {DateTime.Now}", LogSeverity.Information);
            }
        }

        public static void Log(string message, LogSeverity severity)
        {
            foreach (var l in _loggers)
            {
                l.Log(message, severity);
            }
        }
    }
}
