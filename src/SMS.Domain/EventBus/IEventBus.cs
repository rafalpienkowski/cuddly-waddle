using System.Threading.Tasks;

namespace SMS.Domain.EventBus
{
    /// <summary>
    /// Event bus abstraction
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Sends event
        /// 
        /// I assume concrete implementation contains an outbox pattern to guarantee delivery
        /// </summary>
        /// <param name="event"><see cref="IEvent"/></param>
        /// <returns><see cref="Task"/></returns>
        Task Send(IEvent @event);
    }
}