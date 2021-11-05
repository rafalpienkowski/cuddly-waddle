using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Domain;
using SMS.Domain.EventBus;
using SMS.Domain.Events;
using SMS.Domain.ExternalSmsProvider;
using Xunit;

namespace SMS.Service.Tests
{
    public class SendUndeliveredMessagesCommandTests
    {
        private readonly SendUndeliveredMessagesCommand _command = new();

        private readonly Mock<ILogger<SendUndeliveredMessagesHandler>> _logger;
        private readonly Mock<ISendSmsService> _sendSmsService;
        private readonly Mock<IUndeliveredSmsRepository> _undeliveredSmsRepository;
        private readonly Mock<IEventBus> _eventBus;

        private readonly List<UndeliveredMessage> _undeliveredMessages = new List<UndeliveredMessage>
        {
            UndeliveredMessage.Create("+44 500 500 500", "Some text"),
            UndeliveredMessage.Create("+44 500 500 501", "Another text"),
        };

        private readonly SendUndeliveredMessagesHandler _sut;

        public SendUndeliveredMessagesCommandTests()
        {
            _logger = new Mock<ILogger<SendUndeliveredMessagesHandler>>();
            
            _undeliveredSmsRepository = new Mock<IUndeliveredSmsRepository>();
            _undeliveredSmsRepository.Setup(_ => _.GetUndeliveredMessages()).ReturnsAsync(_undeliveredMessages);
            
            _sendSmsService = new Mock<ISendSmsService>();
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Success);
            _eventBus = new Mock<IEventBus>();

            _sut = new SendUndeliveredMessagesHandler(_logger.Object, _undeliveredSmsRepository.Object,
                _sendSmsService.Object, _eventBus.Object);
        }

        [Fact]
        public async Task Handler_Should_Get_All_Undelivered_Messages()
        {
            await _sut.Handle(_command, CancellationToken.None);
            
            _undeliveredSmsRepository.Verify(_ => _.GetUndeliveredMessages(), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Send_Undelivered_Messages()
        {
            await _sut.Handle(_command, CancellationToken.None);

            foreach (var undeliveredMessage in _undeliveredMessages)
            {
                _sendSmsService.Verify(_ => _.Send(It.Is<SmsMessage>(sms =>
                    sms.PhoneNumber == undeliveredMessage.PhoneNumber && sms.SmsText == undeliveredMessage.SmsText)));
            }
        }

        [Fact]
        public async Task Handler_Should_Send_Event_After_Sms_Is_Sent()
        {
            await _sut.Handle(_command, CancellationToken.None);
            
            foreach (var undeliveredMessage in _undeliveredMessages)
            {
                _eventBus.Verify(_ => _.Send(It.Is<SmsSentEvent>(sentEvent =>
                    sentEvent.PhoneNumber == undeliveredMessage.PhoneNumber)));
            }
        }

        [Fact]
        public async Task Handler_Should_Mark_Message_As_Delivered()
        {
            await _sut.Handle(_command, CancellationToken.None);
            
            foreach (var undeliveredMessage in _undeliveredMessages)
            {
                _undeliveredSmsRepository.Verify(_ => _.Save(It.Is<UndeliveredMessage>(message =>
                    message.PhoneNumber == undeliveredMessage.PhoneNumber && message.Delivered)));
            }
        }

        [Fact]
        public async Task Handler_Should_Update_Last_Delivery_Fail_Time_When_Send_Sms_Fails()
        {
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Fail("Service unavailable"));
            
            await _sut.Handle(_command, CancellationToken.None);
            
            foreach (var undeliveredMessage in _undeliveredMessages)
            {
                var firstAttemptMessage = _undeliveredMessages.First(um => um.PhoneNumber == undeliveredMessage.PhoneNumber);
                _undeliveredSmsRepository.Verify(_ => _.Save(It.Is<UndeliveredMessage>(message =>
                    message.PhoneNumber == undeliveredMessage.PhoneNumber &&
                    !message.Delivered &&
                    message.DeliverFailedAt >= firstAttemptMessage.DeliverFailedAt)));
            }
        }

        [Fact]
        public async Task Handler_Should_Update_Undelivered_Message_When_Message_Is_Sent()
        {
            await _sut.Handle(_command, CancellationToken.None);
            
            _undeliveredSmsRepository.Verify(_ => _.Save(It.IsAny<UndeliveredMessage>()));
        }
        
        [Fact]
        public async Task Handler_Should_Update_Undelivered_Message_When_Message_Is_Not_Sent()
        {
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Fail("Service unavailable"));
            
            await _sut.Handle(_command, CancellationToken.None);
            
            _undeliveredSmsRepository.Verify(_ => _.Save(It.IsAny<UndeliveredMessage>()));
        }
    }
}