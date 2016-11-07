# NEventLite - Light weight .NET framework for Event Sourcing with support for custom Event and Snapshot Stores (EventStore, Redis, SQL Server or Custom) written in C#.
---------------------------------
NEventLite makes it easier to implement the event sourcing pattern in your .NET project. It is opinionated and enforces some patterns. The framework is built with support for custom storage providers and event bus architectures in mind. We also provide some popular event/snapshot storage provider implementations for NEventLite here. Feel free to use it as is or customize it to suit your needs.

• There is a seperate Repo for NEventLite targetting .NET Standard framework https://github.com/dasiths/NEventLite_Core

Author: Dasith Wijesiriwardena
----------------------------------
Requirements:

•	A basic understanding of what Event Sourcing is. I recommend watching Greg Young's presentations and speeches about it on YouTube. 
Start with : https://www.youtube.com/watch?v=JHGkaShoyNs

• This purpose of the example project is to demonstrate the Event Sourcing design pattern using the EventStore (https://geteventstore.com/) and .NET

•	Installation of EventStore (Optional, There is a built in InMemoryStorageProvider too)
"Event Store stores your data as a series of immutable events over time, making it easy to build event-sourced applications" - https://geteventstore.com/)

It's very easy to use once setup. Ideal for implementing the CQRS pattern.
------------------------------------
```C#
//See how AggregateRoots, Events and StorageProviders have been setup in the Example project.
//EventStorageProvider and SnapshotStorageProvider can be injected to the Repository.
//Can be created per command or once per life time as follows.

var rep = new NoteRepository(resolver.Resolve<IRepositoryBase<Note>>());
var commandHandler = new NoteCommandHandler(rep);

//Create new note
commandHandler.Handle(new CreateNoteCommand(Guid.NewGuid(), -1, "Test Note","Event Sourcing System Demo", "Event Sourcing"));

//Example of a Command Handler
    public class NoteCommandHandler :
        ICommandHandler<CreateNoteCommand>, 
        ICommandHandler<EditNoteCommand>
    {
        private readonly RepositoryDecorator<Note> _repositoryBase;
        public Guid LastCreatedNoteGuid { get; private set; }

        public NoteCommandHandler(RepositoryDecorator<Note> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public int Handle(CreateNoteCommand command)
        {
            var newNote = new Note(command.title, command.desc, command.cat);
            _repositoryBase.Save(newNote);
            LastCreatedNoteGuid = newNote.Id;
            return 0;
        }

        public int Handle(EditNoteCommand command)
        {
            var LoadedNote = _repositoryBase.GetById(command.AggregateId);

            if (LoadedNote != null)
            {
                if (LoadedNote.CurrentVersion == command.TargetVersion)
                {
                    if (LoadedNote.Title != command.title)
                        LoadedNote.ChangeTitle(command.title);

                    if (LoadedNote.Category != command.cat)
                        LoadedNote.ChangeCategory(command.cat);

                    _repositoryBase.Save(LoadedNote);

                    return LoadedNote.CurrentVersion;
                }
                else
                {
                    throw new ConcurrencyException($"The version of the Note ({LoadedNote.CurrentVersion}) and Command ({command.TargetVersion}) didn't match.");
                }
            }
            else
            {
                throw new AggregateNotFoundException($"Note with ID {command.AggregateId} was not found.");
            }
        }
    }
```

Notes
------------------------------------
Please feel free to contribute and improve the code as you see fit.

What's next?
Have a look at this awesome CQRS tutorial: http://www.codeproject.com/Articles/555855/Introduction-to-CQRS
The event sourcing pattern I implemented is very close to the implementation in the tutorial linked.
