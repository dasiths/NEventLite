using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Logger
{
    public static class LogManager
    {
        private static ILogger _logger;
        public static ILogger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger?.Log($"Logging stopped at: {DateTime.Now}", LogSeverity.Information);
                value?.Log($"Logging started at: {DateTime.Now}", LogSeverity.Information);
                _logger = value;
            }
        }
    }
}
