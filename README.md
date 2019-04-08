# NEventLite [![NuGet](https://img.shields.io/nuget/v/NEventLite.svg)](https://www.nuget.org/packages/NEventLite)
## An extensible lightweight library for .NET that manages the Aggregate lifecycle in an **Event Sourced** system.
Supports `Event` and `Snapshot` storage providers like EventStore/Redis or SQL Server. Built with dependency injection in mind and seamlessly integrates with AspNetCore.

## What is Event Sourcing?

> Use an append-only store to record the full series of events that describe actions taken on data in a domain, rather than storing just the current state.

Start here https://dasith.me/2016/12/02/event-sourcing-examined-part-1-of-3/

## What does NEventLite solve?
NEventLite makes it easier to implement the event sourcing pattern in your .NET project. The library is opinionated and introduces some patterns to manage the life cycle of your [Aggregates](https://martinfowler.com/bliki/DDD_Aggregate.html) in an event sourced system. It will manage Creating, Loading, Mutating and Persisting Aggregates and Events.

## What doesn't it solve?

NEventLite is **not a framework** that manages your application end to end. It doesn't enforce ports and adapters pattern or any of the application level concerns. The aim is to do one thing (Manage aggregate lifecycle) and do that well. If you need to implement command and event handlers you can have a look at something like [SimpleMediator](https://github.com/dasiths/SimpleMediator) or [Brighter](https://github.com/BrighterCommand/Brighter) and NEventLite will complement them nicely.

## What about v1.0? Wasn't it advertised as a framework? 
*NEventLite V1.0 tried to solve simmilar problems but the scope of the project very large and it was decided to narrow down the scope. If you're still looking for reference it's hosted [here](https://github.com/dasiths/NEventLite/blob/master/legacy/v1.0).*

## Before you start

- The library targets .NET Standard 2.0
- *Optional:* Installation of EventStore - https://geteventstore.com/ (You can use the in memory event and snapshot providers when developing)

## Using It

Define the events. They are simple pocos that will be serialized and stored in `EventStorage` when changes are saved. Events use `Guid` for Id by *default* but they can be changed to use any data type as Id. See `Event<TAggregateKey, TEventKey>` for reference.
```csharp
    public class ScheduleCreatedEvent : Event<Schedule>
    {
        public string ScheduleName { get; set; }

        public ScheduleCreatedEvent(Guid scheduleId, string scheduleName) : base(Guid.NewGuid(), scheduleId)
        {
            ScheduleName = scheduleName;
        }
    }

    public class TodoCreatedEvent : Event<Schedule>
    {
        public Guid TodoId { get; set; }
        public string Text { get; set; }

        public TodoCreatedEvent(Guid aggregateId, int targetVersion, Guid todoId, string text) : base(Guid.NewGuid(), aggregateId, targetVersion)
        {
            TodoId = todoId;
            Text = text;
        }
    }
```
Define the Aggregate (`Schedule.cs` in the sample). Aggregates use `Guid` as Id by *default* but just like the events, they can be changed to the Id type of your choice. See `AggregateRoot<TAggregateKey>` for reference.
```csharp
    public class Schedule : AggregateRoot
    {
        public IList<Todo> Todos { get; private set; }
        public string ScheduleName { get; private set; }

        // To create or mutate, call the constructor or methods. 
        
        // Constructor
        public Schedule(string scheduleName)
        {
            // Pattern: Create an event and apply it
            var newScheduleId = Guid.NewGuid();
            var @event = new ScheduleCreatedEvent(newScheduleId, scheduleName);
            ApplyEvent(@event);
        }

        public Guid AddTodo(string text)
        {
            // Pattern: Create an event and apply it
            var newTodoId = Guid.NewGuid();
            var @event = new TodoCreatedEvent(Id, CurrentVersion, newTodoId, text);
            ApplyEvent(@event);
            return newTodoId;
        }
        
        // ** Mutations happen through applying events **
        // The library identifies the internal event handler methods though a special method attribute.
        
        [InternalEventHandler]
        public void OnScheduleCreated(ScheduleCreatedEvent @event)
        {
            ScheduleName = @event.ScheduleName;
            Todos = new List<Todo>();
        }

        [InternalEventHandler]
        public void OnTodoCreated(TodoCreatedEvent @event)
        {
            var todo = new Todo(@event.TodoId, @event.Text);
            Todos.Add(todo);
        }
    }
```

Using the built in `Session` and `Repository` implementations to manage the Aggregate lifecycle.

```csharp
// We recommend using a DI container to inject the Session. Keep it scoped (per request in a web application) 

    public class CreateScheduleHandler
    {
        private readonly Session<Schedule> _session;

        public CreateScheduleCommandHandler(Session<Schedule> session)
        {
            _session = session;
        }

        public async Task<Guid> HandleAsync()
        {
            var schedule = new Schedule("my new schedule");
            _session.Attach(schedule);
            await _session.SaveAsync();
            return schedule.Id;
        }
    }

    public class AddTodoHandler
    {
        private readonly Session<Schedule> _session;

        public AddTodoCommandHandler(Session<Schedule> session)
        {
            _session = session;
        }

        public async Task<Guid> HandleAsync(Guid scheduleId)
        {
            var schedule = await _session.GetByIdAsync(scheduleId);
            var id = schedule.AddTodo("test todo 1");
            await _session.SaveAsync();
            return id;
        }
    }
```

## Storage providers

The library contains storage provider implementation for [EventSore](https://eventstore.org/) and we plan to include a few more in the future. We have also included an in memory event and snapshot storage provider to get you up and running faster.

It's very easy to implement your own as well. Implement `IEventStorageProvider` and `ISnapshotStorageProvider` interfaces an you're good to plug them in. If you need help look at the `NEventLite.StorageProviders.EventStore` project in the repository.

```csharp
    public interface IEventStorageProvider<TAggregate, TAggregateKey, TEventKey> where TAggregate : AggregateRoot<TAggregateKey, TEventKey>
    {
        Task<IEnumerable<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>>> GetEventsAsync(TAggregateKey aggregateId, int start, int count);
        Task<IEvent<AggregateRoot<TAggregateKey, TEventKey>, TAggregateKey, TEventKey>> GetLastEventAsync(TAggregateKey aggregateId);
        Task SaveAsync(AggregateRoot<TAggregateKey, TEventKey> aggregate);
    }

    public interface ISnapshotStorageProvider<TSnapshot, in TAggregateKey, TSnapshotKey> where TSnapshot: ISnapshot<TAggregateKey, TSnapshotKey>
    {
        int SnapshotFrequency { get; }
        Task<TSnapshot> GetSnapshotAsync(TAggregateKey aggregateId);
        Task SaveSnapshotAsync(TSnapshot snapshot);
    }
```

There are more examples in the Samples if you need help figuring out how to put everything together.

## Notes
Please feel free to contribute and improve the code as you see fit. Please raise an issue if you find a bug or have an improvement idea. The repository is shared under the MIT license.
