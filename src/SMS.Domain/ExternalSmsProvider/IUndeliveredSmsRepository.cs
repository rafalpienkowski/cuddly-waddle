using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.Domain.ExternalSmsProvider
{
    public interface IUndeliveredSmsRepository
    {
        Task<Result> Save(UndeliveredMessage undeliveredMessage);

        Task<IList<UndeliveredMessage>> GetUndeliveredMessages();
    }
}