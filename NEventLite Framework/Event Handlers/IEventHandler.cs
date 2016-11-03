/****************************** Class Header ******************************\
Module Name:    <IEventHandler.cs>
Project:        <EventSourcingDemo> [https://github.com/dasiths/EventSourcingDemo]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

This simply has an Apply method to do state changes in the implemented AggregateRoot
\***************************************************************************/

using NEventLite.Events;

namespace NEventLite.Event_Handlers
{
    /// <summary>
    /// Interface to expose the Apply() of event T in an AggregateRoot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<T> where T: Event
    {
        /// <summary>
        /// Apply the event
        /// </summary>
        /// <param name="event">Event</param>
        void Apply(T @event);
    }
}
