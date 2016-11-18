using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Event_Handlers
{
    public interface IEventHandler
    {
        Task HandleEventAsync();
    }
}
