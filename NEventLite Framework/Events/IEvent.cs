/****************************** Class Header ******************************\
Module Name:    <IEvent.cs>
Project:        <NEventLite> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

Interface for all events in the system. Requires an unique eventID
\***************************************************************************/

using System;

namespace NEventLite.Events
{
    /// <summary>
    /// Interface for all events
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Unique event ID
        /// </summary>
        Guid Id { get; }
    }
}
