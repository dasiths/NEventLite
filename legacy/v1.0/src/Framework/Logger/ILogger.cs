using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Logger
{
    public interface ILogger
    {
        void Log(string message, LogSeverity severity);
    }

    public enum LogSeverity
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Critical = 3
    }

}
