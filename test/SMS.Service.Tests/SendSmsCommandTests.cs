using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SMS.Domain;
using SMS.Domain.EventBus;
using SMS.Domain.Events;
using SMS.Domain.ExternalSmsProvider;
using Xunit;

namespace SMS.Service.Tests
{
    public class SendSmsCommandTests
    {
        private readonly SendSmsCommand _command = new()
        {
            Id = Guid.NewGuid(),
            PhoneNumber = "+01 500 500 500",
            SmsText = "Test text message"
        };

        private readonly Mock<ILogger<SendSmsHandler>> _logger;
        private readonly Mock<ISendSmsService> _sendSmsService;
        private readonly Mock<IUndeliveredSmsRepository> _undeliveredSmsRepository;
        private readonly Mock<IEventBus> _eventBus;

        private readonly SendSmsHandler _sut;

        public SendSmsCommandTests()
        {
            _logger = new Mock<ILogger<SendSmsHandler>>();
            
            _sendSmsService = new Mock<ISendSmsService>();
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Success);

            _undeliveredSmsRepository = new Mock<IUndeliveredSmsRepository>();

            _eventBus = new Mock<IEventBus>();

            _sut = new SendSmsHandler(_logger.Object, _sendSmsService.Object, _undeliveredSmsRepository.Object,
                _eventBus.Object);
        }

        [Fact]
        public async Task Handler_Should_Call_External_Sms_Service()
        {
            await _sut.Handle(_command, CancellationToken.None);

            _sendSmsService.Verify(
                _ => _.Send(It.Is<SmsMessage>(sms =>
                    sms.PhoneNumber == _command.PhoneNumber && sms.SmsText == _command.SmsText)), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Send_Sms_In_Sunny_Day_Scenario()
        {
            var result = await _sut.Handle(_command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handler_Should_Publish_Sms_Sent_Event_After_External_Service_Successfully_Send_Message()
        {
            await _sut.Handle(_command, CancellationToken.None);
            
            _eventBus.Verify(_ => _.Send(It.Is<SmsSentEvent>(e => e.PhoneNumber == _command.PhoneNumber)), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Save_Undelivered_Message_For_The_Future()
        {
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Fail("Service unavailable"));
            
            await _sut.Handle(_command, CancellationToken.None);

            _undeliveredSmsRepository.Verify(_ =>
                _.Save(It.Is<UndeliveredMessage>(sms =>
                    sms.PhoneNumber == _command.PhoneNumber && sms.SmsText == _command.SmsText)), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Message_When_External_Sms_Service_Is_Unavailable()
        {
            _sendSmsService.Setup(_ => _.Send(It.IsAny<SmsMessage>())).ReturnsAsync(Result.Fail("Service unavailable"));
            
            var result = await _sut.Handle(_command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Message.Should().NotBeEmpty();
        }
    }
}