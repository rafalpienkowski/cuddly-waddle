using System;

namespace SMS.Domain.ExternalSmsProvider
{
    /// <summary>
    /// Undelivered message
    /// </summary>
    public class UndeliveredMessage
    {
        public Guid Id { get; }
        
        public string PhoneNumber { get; }

        public string SmsText { get; }

        public DateTime DeliverFailedAt { get; private set; }

        public bool Delivered { get; private set; } = false;

        private UndeliveredMessage(string phoneNumber, string smsText)
        {
            Id = Guid.NewGuid();
            PhoneNumber = phoneNumber;
            SmsText = smsText;
            DeliverFailedAt = DateTime.UtcNow;
        }

        public void MarkAsDelivered()
        {
            Delivered = true;
        }

        public void UpdateLastDeliveryFail()
        {
            DeliverFailedAt = DateTime.UtcNow;
        }

        public static UndeliveredMessage Create(string phoneNumber, string smsText) =>
            new(phoneNumber, smsText);
    }
}