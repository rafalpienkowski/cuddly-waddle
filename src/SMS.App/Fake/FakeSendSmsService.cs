using System;
using System.Threading.Tasks;
using SMS.Domain;
using SMS.Domain.ExternalSmsProvider;

namespace SMS.App.Fake
{
    public class FakeSendSmsService: ISendSmsService
    {
        private readonly Random _random = new Random();
        
        public Task<Result> Send(SmsMessage smsMessage)
        {
            var value = _random.Next(0, 10);
            if (value >= 5)
            {
                return Task.FromResult<Result>(Result.Success());
            }
            
            return Task.FromResult<Result>(Result.Fail("Something bad happens"));
        }
    }
}