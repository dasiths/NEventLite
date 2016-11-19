using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Events;
using NEventLite.Event_Handlers;
using NEventLite.Logger;
using NEventLite_Example.Events;

namespace NEventLite_Example.Read_Model
{
    public class MyEventSubscriber : IEventHandler<NoteCreatedEvent>,
                                    IEventHandler<NoteTitleChangedEvent>,
                                    IEventHandler<NoteDescriptionChangedEvent>,
                                    IEventHandler<NoteCategoryChangedEvent>
    {

        private readonly MyReadRepository _repository;

        public MyEventSubscriber(MyReadRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleEventAsync(NoteCreatedEvent @event)
        {
            LogEvent(@event);
        }

        public async Task HandleEventAsync(NoteTitleChangedEvent @event)
        {
            LogEvent(@event);
        }

        public async Task HandleEventAsync(NoteDescriptionChangedEvent @event)
        {
            LogEvent(@event);
        }

        public async Task HandleEventAsync(NoteCategoryChangedEvent @event)
        {
            LogEvent(@event);
        }

        private void LogEvent(IEvent @event)
        {
            LogManager.Log(
            $"Event #{@event.TargetVersion + 1} Received: {@event.GetType().Name} @ {DateTime.Now.ToLongTimeString()}",
            LogSeverity.Information);
        }
    }
}
