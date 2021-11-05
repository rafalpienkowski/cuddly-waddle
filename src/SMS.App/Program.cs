using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMS.App.Fake;
using SMS.App.HostedServices;
using SMS.Domain.EventBus;
using SMS.Domain.ExternalSmsProvider;
using SMS.Service;

namespace SMS.App
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddMediatR(typeof(SendSmsCommand));
                    services.AddSingleton<IEventBus, FakeEventBus>();
                    services.AddSingleton<ISendSmsService, FakeSendSmsService>();
                    services.AddSingleton<IUndeliveredSmsRepository, FakeUndeliveredMessagesRepository>();
                    
                    services.AddHostedService<SmsCommandProducerHostedService>();
                    services.AddHostedService<UndeliveredMessagesSenderHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
        }
    }
}