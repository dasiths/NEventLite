/****************************** Class Header ******************************\
Module Name:    <Note.cs>
Project:        <EventSourcingDemo> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

What is an "AggregateRoot" in DDD? http://martinfowler.com/bliki/DDD_Aggregate.html

The "Note" is an AggregateRoot and implements event handlers and ISnapshottable to make replaying of past events faster
\***************************************************************************/

using System;
using NEventLite.Custom_Attributes;
using NEventLite.Domain;
using NEventLite.Snapshot;
using NEventLite_Example.Events;
using NEventLite_Example.Snapshot;

namespace NEventLite_Example.Domain
{
    /// <summary>
    /// Note is an AggregateRoot. 
    /// Implements IEventHandler for NoteCreatedEvent, NoteTitleChangedEvent, NoteCategoryChangedEvent.
    /// Implements ISnashottable.
    /// </summary>
    public class Note : AggregateRoot, ISnapshottable
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
            //Important: Aggregate roots must have a blank constructor
        }

        /// <summary>
        /// Constructor with some parameters
        /// </summary>
        /// <param name="id">ID of new Note</param>
        /// <param name="title">Title</param>
        /// <param name="desc">Description</param>
        /// <param name="cat">Category</param>
        public Note(Guid id, string title, string desc, string cat):this()
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            ApplyEvent(new NoteCreatedEvent(id, CurrentVersion, title, desc, cat, DateTime.Now));
        }

        /// <summary>
        /// Change the title of the note
        /// </summary>
        /// <param name="newTitle">New Title</param>
        public void ChangeTitle(string newTitle)
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            ApplyEvent(new NoteTitleChangedEvent(Id, CurrentVersion, newTitle));
        }

        /// <summary>
        /// Change category of the note
        /// </summary>
        /// <param name="newCategory">New Category</param>
        public void ChangeCategory(string newCategory)
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            ApplyEvent(new NoteCategoryChangedEvent(Id, CurrentVersion, newCategory));
        }

        #endregion

        #region "Apply Events"

        [EventHandlingMethod()]
        public void OnNoteCreated(NoteCreatedEvent @event)
        {
            CreatedDate = @event.createdTime;
            Title = @event.title;
            Description = @event.desc;
            Category = @event.cat;
        }

        [EventHandlingMethod()]
        public void OnTitleChanged(NoteTitleChangedEvent @event)
        {
            Title = @event.title;
        }

        [EventHandlingMethod()]
        public void OnCategoryChanged(NoteCategoryChangedEvent @event)
        {
            Category = @event.cat;
        }

        #endregion

        #region "Snapshots"

        /// <summary>
        /// Get new snapshot of the current state of the Aggregate
        /// </summary>
        /// <returns>A snapshot of the Aggregate</returns>
        public NEventLite.Snapshot.Snapshot TakeSnapshot()
        {
            return new NoteSnapshot(Guid.NewGuid(),
                                    Id,
                                    CurrentVersion,
                                    CreatedDate,
                                    Title,
                                    Description,
                                    Category);
        }

        /// <summary>
        /// Applies the snapshot and updates state of the Aggregate
        /// </summary>
        /// <param name="snapshot"></param>
        public void ApplySnapshot(NEventLite.Snapshot.Snapshot snapshot)
        {
            //Important: State changes are done here.
            //Make sure you set the CurrentVersion and LastCommittedVersions here too

            NoteSnapshot item = (NoteSnapshot)snapshot;

            Id = item.AggregateId;
            CurrentVersion = item.Version;
            LastCommittedVersion = item.Version;
            CreatedDate = item.CreatedDate;
            Title = item.Title;
            Description = item.Description;
            Category = item.Category;
        }
        #endregion

    }
}
