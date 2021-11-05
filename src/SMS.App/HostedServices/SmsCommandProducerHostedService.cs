using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMS.Service;

namespace SMS.App.HostedServices
{
    public class SmsCommandProducerHostedService : IHostedService
    {
        private readonly ILogger<SmsCommandProducerHostedService> _logger;
        private readonly IMediator _mediator;
        private Timer _timer;

        public SmsCommandProducerHostedService(ILogger<SmsCommandProducerHostedService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SMS command producer");

            _timer = new Timer(FakeCommand, null, 1000, 5000);
            
            return Task.CompletedTask;
        }

        private void FakeCommand(object state)
        {
            _ = SendCommand();
        }

        private async Task SendCommand()
        {
            var command = new SendSmsCommand
            {
                Id = Guid.NewGuid(),
                PhoneNumber = "+44 500 500 500",
                SmsText = $"Some random text: {Guid.NewGuid()}"
            };
            
            _logger.LogInformation("New SMS command arrived");
            
            await _mediator.Send(command);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping SMS command producer");
            
            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }
    }
}
