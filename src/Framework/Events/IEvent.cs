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
        /// Target version of the Aggregate this event will be applied against
        /// </summary>
        int TargetVersion { get; set; }
        
        /// <summary>
        /// The aggregateID of the aggregate
        /// </summary>
        Guid AggregateId { get; set; }

        /// <summary>
        /// Unique event ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// This is used to timestamp the event when it get's committed
        /// </summary>
        DateTime EventCommittedTimestamp { get; set; }

        /// <summary>
        /// This is used to handle versioning of events over time when refactoring or feature additions are done
        /// </summary>
        int ClassVersion { get; set; }
    }
}
