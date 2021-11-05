using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using SMS.Domain;
using SMS.Domain.ExternalSmsProvider;

namespace SMS.App.Fake
{
    public class FakeUndeliveredMessagesRepository : IUndeliveredSmsRepository
    {
        private readonly IList<UndeliveredMessage> _undeliveredMessages = new List<UndeliveredMessage>();

        public Task<Result> Save(UndeliveredMessage undeliveredMessage)
        {
            var existingMessage = _undeliveredMessages.FirstOrDefault(message => message.Id == undeliveredMessage.Id);
            if(existingMessage != null)
            {
                _undeliveredMessages.Remove(existingMessage);
            }
            
            _undeliveredMessages.Add(undeliveredMessage);

            return Task.FromResult(Result.Success());
        }

        public Task<IList<UndeliveredMessage>> GetUndeliveredMessages() =>
            Task.FromResult<IList<UndeliveredMessage>>(_undeliveredMessages.Where(message => !message.Delivered)
                .ToList());
    }
}