using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingDemo.Events;
using EventSourcingDemo.Event_Handlers;
using EventSourcingDemo.Snapshot;

namespace EventSourcingDemo.Domain
{
    public class Note : AggregateRoot,
                IEventHandler<NoteCreatedEvent>, IEventHandler<NoteTitleChangedEvent>, IEventHandler<NoteCategoryChangedEvent>,
                ISnapshottable
    {
        public DateTime CreatedDate { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }

        #region "Constructor and Methods"

        public Note()
        {

        }

        public Note(string title, string desc, string cat)
        {
            HandleEvent(new NoteCreatedEvent(new Guid(), 0, title, desc, cat, DateTime.Now));
        }

        public void ChangeTitle(string newTitle)
        {
            HandleEvent(new NoteTitleChangedEvent(this.Id, this.CurrentVersion, newTitle));
        }

        public void ChangeCategory(string newCategory)
        {
            HandleEvent(new NoteCategoryChangedEvent(this.Id, this.CurrentVersion, newCategory));
        }

        #endregion

        #region "Apply Events"

        public void Apply(NoteCreatedEvent @event)
        {
            ApplyGenericEvent(@event, true);

            this.CreatedDate = @event.createdTime;
            this.Title = @event.title;
            this.Description = @event.desc;
            this.Category = @event.cat;
        }

        public void Apply(NoteTitleChangedEvent @event)
        {
            ApplyGenericEvent(@event, false);

            this.Title = @event.title;
        }

        public void Apply(NoteCategoryChangedEvent @event)
        {
            ApplyGenericEvent(@event, false);

            this.Category = @event.cat;
        }

        #endregion

        #region "Snapshots"
        public Snapshot.Snapshot GetSnapshot()
        {
            return new NoteSnapshot(new Guid(),
                                    this.Id,
                                    this.CurrentVersion,
                                    this.CreatedDate,
                                    this.Title,
                                    this.Description,
                                    this.Category);
        }

        public void SetSnapshot(Snapshot.Snapshot snapshot)
        {
            NoteSnapshot item = (NoteSnapshot)snapshot;

            this.Id = item.AggregateId;
            this.CurrentVersion = item.Version;
            this.LastCommittedVersion = item.Version;
            this.CreatedDate = item.CreatedDate;
            this.Title = item.Title;
            this.Description = item.Description;
            this.Category = item.Category;
        }
        #endregion

    }
}
