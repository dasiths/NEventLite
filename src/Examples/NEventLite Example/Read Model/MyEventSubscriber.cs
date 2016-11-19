using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Event_Handlers;
using NEventLite_Example.Events;

namespace NEventLite_Example.Read_Model
{
    public class MyEventSubscriber: IEventHandler<NoteCreatedEvent>,
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
            throw new NotImplementedException();
        }

        public async Task HandleEventAsync(NoteTitleChangedEvent @event)
        {
            throw new NotImplementedException();
        }

        public async Task HandleEventAsync(NoteDescriptionChangedEvent @event)
        {
            throw new NotImplementedException();
        }

        public async Task HandleEventAsync(NoteCategoryChangedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
