using System;

namespace SMS.Domain.EventBus
{
    /// <summary>
    /// Base event inforation
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Event's creation date
        /// </summary>
        public DateTime Created { get; }
        
        /// <summary>
        /// Event's unique id
        /// </summary>
        public Guid Id { get; }
    }
}