using DPL.Services.Contract;
using DPL.Services.Helpers;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DPL.Services.Implementation
{
	public class SmsSender : ISmsSender
	{
		private readonly TwilioSettings _twilioSettings;
		public SmsSender(IOptions<TwilioSettings> twilioOptions)
		{
			_twilioSettings = twilioOptions.Value;
		}

		public async Task<MessageResource> SendSmsAsync(SmsMessage sms)
		{
			TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);
			var result = await MessageResource.CreateAsync
				(
				from: new PhoneNumber(_twilioSettings.TwilioPhoneNumber),
				to: new PhoneNumber(sms.PhoneNumber),
				body: sms.Message
				);
			return result;
		}
	}
}
