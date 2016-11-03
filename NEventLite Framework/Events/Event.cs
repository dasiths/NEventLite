/****************************** Class Header ******************************\
Module Name:    <Event.cs>
Project:        <NEventLite> [https://github.com/dasiths/NEventLite]
Author:         Dasith Wijesiriwardena [https://github.com/dasiths]

This is the base class for all events. We implement the reqauired properties to make it work with Aggregates. 
This has be Serializable to make it persist to storage and reload. 
Important: Events are immutable by design. I've left the property set methods public because of potential deserialization issues.
\***************************************************************************/

using System;

namespace NEventLite.Events
{

    /// <summary>
    /// The base class for all events applies to an AggregateRoot
    /// </summary>
    [Serializable]
    public class Event : IEvent
    {
        /// <summary>
        /// Target version of the Aggregate this event will be applied against
        /// </summary>
        public int TargetVersion { get; set; }
        /// <summary>
        /// The aggregateID of the aggregate
        /// </summary>
        public Guid AggregateId { get;  set; }
        /// <summary>
        /// The Unique Event ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// This method makes it easier to contruct a new Event
        /// </summary>
        /// <param name="AggregateID">Aggregate ID</param>
        /// <param name="TargetVersion">Target Version</param>
        protected void Setup(Guid AggregateID, int TargetVersion)
        {
            this.AggregateId = AggregateID;
            this.TargetVersion = TargetVersion;
            this.Id = Guid.NewGuid();
        }
    }

}
