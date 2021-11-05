using System;
using SMS.Domain.EventBus;

namespace SMS.Domain.Events
{
    /// <summary>
    /// Sms sent event
    /// </summary>
    public class SmsSentEvent : IEvent
    {
        public SmsSentEvent(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            Created = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public string PhoneNumber { get; }
        public DateTime Created { get; }
        public Guid Id { get; }
    }
}