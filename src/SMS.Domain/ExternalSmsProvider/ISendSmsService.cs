using System.Threading.Tasks;

namespace SMS.Domain.ExternalSmsProvider
{
    public interface ISendSmsService
    {
        Task<Result> Send(SmsMessage smsMessage);
    }
}