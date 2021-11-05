using System.Threading.Tasks;
using SMS.Domain.EventBus;

namespace SMS.App.Fake
{
    public class FakeEventBus : IEventBus
    {
        public Task Send(IEvent @event) => Task.CompletedTask;
    }
}