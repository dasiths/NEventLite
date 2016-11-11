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
    /// Implements ISnashottable.
    /// </summary>
    public class Note : AggregateRoot, ISnapshottable
    {
        public DateTime CreatedDate { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }

        #region "Constructor and Methods"

        public Note()
        {
            //Important: Aggregate roots must have a parameterless constructor
            //to make it easier to construct from scratch.

            //The very first event in an aggregate is the creation event 
            //which will be applied to an empty object created via this constructor
        }

        //The following method are how external command interact with our aggregate
        //A command will result in following methods being executed and resulting events will be fired

        public Note(Guid id, string title, string desc, string cat):this()
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            ApplyEvent(new NoteCreatedEvent(id, CurrentVersion, title, desc, cat, DateTime.Now));
        }

        public void ChangeTitle(string newTitle)
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            if (this.Title != newTitle)
            {
                ApplyEvent(new NoteTitleChangedEvent(Id, CurrentVersion, newTitle));
            }
        }

        public void ChangeCategory(string newCategory)
        {
            if (this.Category != newCategory)
            {
                ApplyEvent(new NoteCategoryChangedEvent(Id, CurrentVersion, newCategory));
            }
        }

        #endregion

        #region "Apply Events"

        //Important
        //We mark the EventHandler method with the [OnApplyEvent()] marker
        //This way the framework knows which method to invoke when a event happens

        [OnApplyEvent]
        public void OnNoteCreated(NoteCreatedEvent @event)
        {
            CreatedDate = @event.createdTime;
            Title = @event.title;
            Description = @event.desc;
            Category = @event.cat;
        }

        [OnApplyEvent]
        public void OnTitleChanged(NoteTitleChangedEvent @event)
        {
            Title = @event.title;
        }

        [OnApplyEvent]
        public void OnCategoryChanged(NoteCategoryChangedEvent @event)
        {
            Category = @event.cat;
        }

        #endregion

        #region "Snapshots"

        public NEventLite.Snapshot.Snapshot TakeSnapshot()
        {
            //This method returns a snapshot which will be used to reconstruct the state

            return new NoteSnapshot(Guid.NewGuid(),
                                    Id,
                                    CurrentVersion,
                                    CreatedDate,
                                    Title,
                                    Description,
                                    Category);
        }

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
