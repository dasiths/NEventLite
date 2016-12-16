using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Message
{
    public interface IMessage
    {
        /// <summary>
        /// Unique message ID
        /// </summary>
        Guid CorrelationId { get; }
    }
}
