# NEventLite - Light weight .NET framework for Event Sourcing with support for custom Event and Snapshot Stores (EventStore, Redis, SQL Server or Custom) written in C#.
---------------------------------
NEventLite makes it easier to implement the event sourcing pattern in your .NET project. It is opinionated and enforces some patterns. The framework is built with support for custom storage providers and event bus architectures in mind. We also provide some popular event/snapshot storage provider implementations for NEventLite here. Feel free to use it as is or customize it to suit your needs.

Author: Dasith Wijesiriwardena
----------------------------------
Requirements:

•	A basic understanding of what Event Sourcing is. I recommend watching Greg Young's presentations and speeches about it on YouTube. 
Start with : https://www.youtube.com/watch?v=JHGkaShoyNs

• This purpose of the example project is to demonstrate the Event Sourcing design pattern using the EventStore (https://geteventstore.com/) and .NET

•	Installation of EventStore (Optional, There is a built in InMemoryStorageProvider too)
"Event Store stores your data as a series of immutable events over time, making it easy to build event-sourced applications" - https://geteventstore.com/)

Notes
------------------------------------
Please feel free to contribute and improve the code as you see fit.

What's next?
Have a look at this awesome CQRS tutorial: http://www.codeproject.com/Articles/555855/Introduction-to-CQRS
The event sourcing pattern I implemented is very close to the implementation in the tutorial linked.
