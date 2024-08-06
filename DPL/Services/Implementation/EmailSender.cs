using DPL.Services.Contract;
using DPL.Services.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DPL.Services.Implementation
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _emailSettings;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(IOptions<EmailSettings> emailSettings,
			ILogger<EmailSender> logger)
		{
			_emailSettings = emailSettings.Value;
			_logger = logger;
		}

		public async Task SendEmailAsync(Email email)
		{
			#region Preparing MimeMessage

			var mimeMessage = new MimeMessage()
			{
				Sender = MailboxAddress.Parse(_emailSettings.SenderEmail),
				Subject = email.Subject,
			};

			mimeMessage.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.SenderEmail));
			mimeMessage.To.Add(MailboxAddress.Parse(email.To));

			var body = new BodyBuilder() { TextBody = email.Body };

			if (email.Attachments is not null)
			{
				byte[] bytes;

				foreach (var file in email.Attachments)
				{
					if (file.Length > 0)
					{
						using var memoryStream = new MemoryStream();
						file.CopyTo(memoryStream);
						bytes = memoryStream.ToArray();

						body.Attachments.Add(file.FileName, bytes);
					}
				}
			}
			mimeMessage.Body = body.ToMessageBody();

			#endregion

			#region Connecting & Sending Email

			using var smtpClient = new SmtpClient();
			try
			{
				await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
				await smtpClient.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
				await smtpClient.SendAsync(mimeMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An Error Occurred While Sending Email");
				throw;
			}
			finally
			{
				await smtpClient.DisconnectAsync(true);
			}

			#endregion
		}
	}
}
