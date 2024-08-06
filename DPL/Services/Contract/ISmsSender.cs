using DPL.Services.Helpers;
using Twilio.Rest.Api.V2010.Account;

namespace DPL.Services.Contract
{
	public interface ISmsSender
	{
		public Task<MessageResource> SendSmsAsync(SmsMessage sms);
	}
}
