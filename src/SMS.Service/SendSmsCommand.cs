using System;
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
    public class SendSmsCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; }

        public string SmsText { get; set; }
    }
    
    public class SendSmsHandler : IRequestHandler<SendSmsCommand, Result>
    {
        private readonly ILogger<SendSmsHandler> _logger;
        private readonly ISendSmsService _sendSmsService;
        private readonly IUndeliveredSmsRepository _undeliveredSmsRepository;
        private readonly IEventBus _eventBus;

        public SendSmsHandler(
            ILogger<SendSmsHandler> logger,
            ISendSmsService sendSmsService, 
            IUndeliveredSmsRepository undeliveredSmsRepository, 
            IEventBus eventBus)
        {
            _logger = logger;
            _sendSmsService = sendSmsService;
            _undeliveredSmsRepository = undeliveredSmsRepository;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(SendSmsCommand request, CancellationToken cancellationToken)
        {
            var smsMessage = new SmsMessage
            {
                PhoneNumber = request.PhoneNumber,
                SmsText = request.SmsText
            };
            
            _logger.LogInformation("Sending SMS via external service");
            var result = await _sendSmsService.Send(smsMessage);

            if (result.IsFailure)
            {
                var undeliveredMessage = UndeliveredMessage.Create(smsMessage.PhoneNumber, smsMessage.SmsText);
                
                _logger.LogInformation($"Saving undelivered message for phone: {undeliveredMessage.PhoneNumber}");
                await _undeliveredSmsRepository.Save(undeliveredMessage);
                
                return result;
            }
            
            var smsSentEvent = new SmsSentEvent(request.PhoneNumber);
            
            _logger.LogInformation("Notifying that SMS has been sent");
            await _eventBus.Send(smsSentEvent);

            return result;
        }
    }
}