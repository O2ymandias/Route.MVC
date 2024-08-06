using DPL.Services.Helpers;

namespace DPL.Services.Contract
{
	public interface IEmailSender
	{
		Task SendEmailAsync(Email email);
	}
}
