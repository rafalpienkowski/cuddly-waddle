using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SMS.Domain;
using SMS.Domain.EventBus;
using SMS.Domain.Events;
using SMS.Domain.ExternalSmsProvider;

namespace SMS.Service
{
    public class SendUndeliveredMessagesCommand : IRequest<Result>
    {
    }
    
    public class SendUndeliveredMessagesHandler : IRequestHandler<SendUndeliveredMessagesCommand, Result>
    {
        private readonly ILogger<SendUndeliveredMessagesHandler> _logger;
        private readonly IUndeliveredSmsRepository _undeliveredSmsRepository;
        private readonly ISendSmsService _sendSmsService;
        private readonly IEventBus _eventBus;

        public SendUndeliveredMessagesHandler(
            ILogger<SendUndeliveredMessagesHandler> logger,
            IUndeliveredSmsRepository undeliveredSmsRepository, 
            ISendSmsService sendSmsService, 
            IEventBus eventBus)
        {
            _logger = logger;
            _undeliveredSmsRepository = undeliveredSmsRepository;
            _sendSmsService = sendSmsService;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(SendUndeliveredMessagesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Gathering undelivered messages");
            var undeliveredMessages = await _undeliveredSmsRepository.GetUndeliveredMessages();
            _logger.LogInformation($"Found {undeliveredMessages.Count()} undelivered messages");
            
            foreach (var undeliveredMessage in undeliveredMessages)
            {
                var smsMessage = new SmsMessage
                {
                    PhoneNumber = undeliveredMessage.PhoneNumber,
                    SmsText = undeliveredMessage.SmsText
                };

                _logger.LogInformation($"Retrying to send SMS to {smsMessage.PhoneNumber} for {undeliveredMessage.Id}");
                var result = await _sendSmsService.Send(smsMessage);

                if (result.IsFailure)
                {
                    _logger.LogInformation($"Updating delivery failed for message: {undeliveredMessage.Id}");
                    undeliveredMessage.UpdateLastDeliveryFail();
                }
                else
                {
                    _logger.LogInformation($"Marking as delivered message: {undeliveredMessage.Id}");
                    undeliveredMessage.MarkAsDelivered();
                    
                    var smsSentEvent = new SmsSentEvent(undeliveredMessage.PhoneNumber);

                    _logger.LogInformation("Notyfing that SMS has been sent");
                    await _eventBus.Send(smsSentEvent);
                }
                
                _logger.LogInformation($"Updating undelivered message {undeliveredMessage.Id}");
                await _undeliveredSmsRepository.Save(undeliveredMessage);
            }

            return Result.Success();
        }
    }
}