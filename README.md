# NEventLite - A lightweight .NET framework for Event Sourcing with support for custom Event and Snapshot Stores (EventStore, Redis, SQL Server or Custom) written in C#.
---------------------------------
NEventLite makes it easier to implement the event sourcing pattern in your .NET project. It is opinionated and enforces some patterns. The framework is built with support for custom storage providers and event bus architectures in mind. We also provide some popular event/snapshot storage provider implementations for NEventLite here. Feel free to use it as is or customize it to suit your needs.

####DISCLAIMER: You generally want to avoid "frameworks" when implementing CQRS or Event Sourcing but NEventLite will help get you on the rails. You can swap out every aspect of it if need be. Think of it as training wheels.

• The framework (NEventLite.csproj) targets netstandard 1.4
• Example projects target .net framework 4.6.1
• This purpose of this NEventLite is to demonstrate the Event Sourcing design pattern using .NET

Author: Dasith Wijesiriwardena
----------------------------------
Requirements:

• A basic understanding of what Event Sourcing is. Start here https://dasith.me/2016/12/02/event-sourcing-examined-part-1-of-3/

• Installation of EventStore (Optional, There is a built in InMemoryStorageProvider too)
"Event Store stores your data as a series of immutable events over time, making it easy to build event-sourced applications" - https://geteventstore.com/)

It's very easy to use once setup. Ideal for implementing the CQRS pattern.
------------------------------------
```C#
//See how AggregateRoots, Events and StorageProviders have been setup in the Example project.
//EventStorageProvider and SnapshotStorageProvider can be injected to the Repository.
//Can be created per command or once per life time as follows.

//Load dependency resolver

async Task CreateNote() {
    using (var container = new DependencyResolver())
    {
        //Get ioc container to create our command bus
        var commandBus = container.Resolve<ICommandBus>();        

        //Create new note by sending command to the Command Bus
        Guid itemId = Guid.NewGuid();
        
        var publishResult = await commandBus.ExecuteAsync(
                     new CreateNoteCommand(Guid.NewGuid(), newItemId, -1,
                     "Test Note", "Event Sourcing System Demo", "Event Sourcing"));	   

		//This will throw an exception if the command was not published
		//You can add logic to retry the command as required
		publishResult.EnsurePublished();
    }
}

```
Command Handler (NoteCommandHandler.cs in example)

```C#
        public async Task HandleCommandAsync(CreateNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var newNote = new Note(command.AggregateId, command.Title, command.Desc, command.Cat);
            work.Add(newNote);

            await work.CommitAsync();
        }

        public async Task HandleCommandAsync(EditNoteCommand command)
        {
            var work = new UnitOfWork(_repository);
            var loadedNote = await work.GetAsync<Note>(command.AggregateId, command.TargetVersion);

            loadedNote.ChangeTitle(command.Title);
            loadedNote.ChangeDescription(command.Description);
            loadedNote.ChangeCategory(command.Cat);

            await work.CommitAsync();
        }
```
Aggregate (Note.cs in example)

```C#
        //Commands call the constructor or methods. Which creates an event and applies it to the Aggregate.
        
        public Note(Guid id, string title, string desc, string cat):this()
        {
            //Pattern: Create the event and call ApplyEvent(Event)
            ApplyEvent(new NoteCreatedEvent(id, this.CurrentVersion, title, desc, cat, DateTime.Now));
        }    

        public void ChangeTitle(string newTitle)
        {
            ApplyEvent(new NoteTitleChangedEvent(this.Id, this.CurrentVersion, newTitle));
        }
        
        //Applying Events 
        //Note how the framework identifies the internal event handler methods though a method attribute.
        
        [InternalEventHandler]
        public void OnNoteCreated(NoteCreatedEvent @event)
        {
            CreatedDate = @event.createdTime;
            Title = @event.title;
            Description = @event.desc;
            Category = @event.cat;
        }

        [InternalEventHandler]
        public void OnTitleChanged(NoteTitleChangedEvent @event)
        {
            Title = @event.title;
        }
```

Implement IEventStorageProvider and ISnapshotStorage provider for storage or use the ones we provide for popular stores. See "Storage Providers" project for ready to use implementations for EventStore and Redis. More will be added.

```C#
    public interface IEventStorageProvider
    {
        Task<IEnumerable<IEvent>> GetEventsAsync(Type aggregateType, Guid aggregateId, int start, int count);
        Task<IEvent> GetLastEventAsync(Type aggregateType, Guid aggregateId);
        Task CommitChangesAsync(AggregateRoot aggregate);
    }

	public interface ISnapshotStorageProvider
    {
        int SnapshotFrequency { get; }
        Task<Snapshot.Snapshot> GetSnapshotAsync(Type aggregateType, Guid aggregateId);
        Task SaveSnapshotAsync(Type aggregateType, Snapshot.Snapshot snapshot);
    }
```

Notes
------------------------------------
Please feel free to contribute and improve the code as you see fit.
