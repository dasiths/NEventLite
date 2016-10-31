/****************************** Class Header ******************************\
Module Name:    <Note.cs>
Project:        <EventSourcingDemo> [https://github.com/dasiths/EventSourcingDemo]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

What is an "AggregateRoot" in DDD? http://martinfowler.com/bliki/DDD_Aggregate.html

The "Note" is an AggregateRoot and implements event handlers and ISnapshottable to make replaying of past events faster
\***************************************************************************/

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
    /// <summary>
    /// Note is an AggregateRoot. 
    /// Implements IEventHandler for NoteCreatedEvent, NoteTitleChangedEvent, NoteCategoryChangedEvent.
    /// Implements ISnashottable.
    /// </summary>
    public class Note : AggregateRoot,
                IEventHandler<NoteCreatedEvent>, IEventHandler<NoteTitleChangedEvent>, IEventHandler<NoteCategoryChangedEvent>,
                ISnapshottable
    {
        public DateTime CreatedDate { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }

        #region "Constructor and Methods"

        /// <summary>
        /// Blank constructor as required
        /// </summary>
        public Note()
        {
            //Important: Aggregte roots must have a blank constructor
        }

        /// <summary>
        /// Constructor with some parameters
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="desc">Description</param>
        /// <param name="cat">Category</param>
        public Note(string title, string desc, string cat)
        {
            //Pattern: Create the event and call HandleEvent(Event)
            HandleEvent(new NoteCreatedEvent(Guid.NewGuid(), 0, title, desc, cat, DateTime.Now));
        }

        /// <summary>
        /// Change the title of the note
        /// </summary>
        /// <param name="newTitle">New Title</param>
        public void ChangeTitle(string newTitle)
        {
            //Pattern: Create the event and call HandleEvent(Event)
            HandleEvent(new NoteTitleChangedEvent(this.Id, this.CurrentVersion, newTitle));
        }

        /// <summary>
        /// Chnage category of the note
        /// </summary>
        /// <param name="newCategory">New Category</param>
        public void ChangeCategory(string newCategory)
        {
            //Pattern: Create the event and call HandleEvent(Event)
            HandleEvent(new NoteCategoryChangedEvent(this.Id, this.CurrentVersion, newCategory));
        }

        #endregion

        #region "Apply Events"

        /// <summary>
        /// Apply the NoteCreatedEvent. Apply() is of IEventHandler(of NoteCreatedEvent)
        /// </summary>
        /// <param name="event">Event to apply</param>
        public void Apply(NoteCreatedEvent @event)
        {
            //Important: State change done here
            //Pattern: Apply the generic event details first
            ApplyGenericEvent(@event, true);

            // Then apply specific event details
            this.CreatedDate = @event.createdTime;
            this.Title = @event.title;
            this.Description = @event.desc;
            this.Category = @event.cat;
        }

        /// <summary>
        /// Apply the NoteTitleChanged. Apply() is of IEventHandler(of NoteTitleChangedEvent)
        /// </summary>
        /// <param name="event">Event to apply</param>
        public void Apply(NoteTitleChangedEvent @event)
        {
            //Important: State change done here
            //Pattern: Apply the generic event details first
            ApplyGenericEvent(@event, false);

            // Then apply specific event details
            this.Title = @event.title;
        }

        /// <summary>
        /// Apply the NoteCategoryChangedEvent. Apply() is of IEventHandler( of NoteCategoryChangedEvent)
        /// </summary>
        /// <param name="event">Event to apply</param>
        public void Apply(NoteCategoryChangedEvent @event)
        {
            //Important: State change done here
            //Pattern: Apply the generic event details first
            ApplyGenericEvent(@event, false);

            // Then apply specific event details
            this.Category = @event.cat;
        }

        #endregion

        #region "Snapshots"

        /// <summary>
        /// Get new snapshot of the current state of the Aggregate
        /// </summary>
        /// <returns>A snapshot of the Aggregate</returns>
        public Snapshot.Snapshot GetSnapshot()
        {
            return new NoteSnapshot(Guid.NewGuid(),
                                    this.Id,
                                    this.CurrentVersion,
                                    this.CreatedDate,
                                    this.Title,
                                    this.Description,
                                    this.Category);
        }

        /// <summary>
        /// Applies the snapshot and updates state of the Aggregate
        /// </summary>
        /// <param name="snapshot"></param>
        public void SetSnapshot(Snapshot.Snapshot snapshot)
        {
            //Important: State changes are done here.
            //Make sure you set the CurrentVersion and LastCommittedVersions here too

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
