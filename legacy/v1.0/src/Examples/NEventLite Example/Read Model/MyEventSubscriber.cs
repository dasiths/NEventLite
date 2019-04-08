using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Event;
using NEventLite.Event_Handler;
using NEventLite.Logger;
using NEventLite_Example.Event;

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

            _repository.AddNote(new NoteReadModel(@event.AggregateId, @event.CreatedTime, @event.Title, @event.Desc, @event.Cat));
        }

        public async Task HandleEventAsync(NoteTitleChangedEvent @event)
        {
            LogEvent(@event);

            var note = _repository.GetNote(@event.AggregateId);
            note.CurrentVersion = @event.TargetVersion + 1;
            note.Title = @event.Title;

            _repository.SaveNote(note);
        }

        public async Task HandleEventAsync(NoteDescriptionChangedEvent @event)
        {
            LogEvent(@event);

            var note = _repository.GetNote(@event.AggregateId);
            note.CurrentVersion = @event.TargetVersion + 1;
            note.Description = @event.Description;

            _repository.SaveNote(note);
        }

        public async Task HandleEventAsync(NoteCategoryChangedEvent @event)
        {
            LogEvent(@event);

            var note = _repository.GetNote(@event.AggregateId);
            note.CurrentVersion = @event.TargetVersion + 1;
            note.Category = @event.Cat;

            _repository.SaveNote(note);
        }

        private void LogEvent(IEvent @event)
        {
            LogManager.Log(
            $"Event #{@event.TargetVersion + 1} Received: {@event.GetType().Name} @ {DateTime.Now.ToLongTimeString()}",
            LogSeverity.Information);
        }
    }
}
