using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMS.Service;

namespace SMS.App.HostedServices
{
    public class UndeliveredMessagesSenderHostedService : IHostedService
    {
        private readonly ILogger<UndeliveredMessagesSenderHostedService> _logger;
        private readonly IMediator _mediator;
        private Timer _timer;

        public UndeliveredMessagesSenderHostedService(ILogger<UndeliveredMessagesSenderHostedService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting undelivered message sender");
            
            _timer = new Timer(CleanUndeliveredMessages, null, 1000, 15000);
            
            return Task.CompletedTask;
        }

        private void CleanUndeliveredMessages(object state)
        {
            _logger.LogInformation("Start sending undelivered messages");
            
            _ = SendCommand();
        }

        private async Task SendCommand()
        {
            var command = new SendUndeliveredMessagesCommand();
            
            _logger.LogInformation("Send undelivered messages command");

            await _mediator.Send(command);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping undelivered message sender");
            
            _timer?.Change(Timeout.Infinite, 0);
            
            return Task.CompletedTask;
        }
    }
}