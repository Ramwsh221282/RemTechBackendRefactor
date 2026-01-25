using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;
using RemTech.SharedKernel.Core.PrimitivesModule.Immutable;

namespace Notifications.Infrastructure.EmailSending;

public sealed class EmailSender(Serilog.ILogger logger)
{
	private const int Port = 587;
	private Serilog.ILogger Logger { get; } = logger.ForContext<EmailSender>();

	public async Task<bool> Process(
		Mailer mailer,
		PendingEmailNotification notification,
		CancellationToken ct = default
	)
	{
		ImmutableString smtpHost = new(mailer.Credentials.SmtpHost);
		ImmutableString smtpPassword = new(mailer.Credentials.SmtpPassword);
		ImmutableString senderEmail = new(mailer.Credentials.Email);
		BodyBuilder builder = new() { TextBody = notification.Body };
		MimeMessage mimeMessage = new();
		mimeMessage.Subject = notification.Subject;
		mimeMessage.Body = builder.ToMessageBody();
		mimeMessage.To.Add(MailboxAddress.Parse(notification.Recipient));
		mimeMessage.From.Add(MailboxAddress.Parse(senderEmail.Read()));
		using SmtpClient client = new();
		try
		{
			client.AuthenticationMechanisms.Add("XOAUTH2");
			await client.ConnectAsync(smtpHost.Read(), Port, SecureSocketOptions.StartTls, ct);
			await client.AuthenticateAsync(senderEmail.Read(), smtpPassword.Read(), ct);
			await client.SendAsync(mimeMessage, ct);
			await client.DisconnectAsync(true, ct);
			Logger.Information("Email sent successfully. Recipient: {Recipient}", notification.Recipient);
			return true;
		}
		catch (Exception e)
		{
			Logger.Fatal(e, "Error sending email. Recipient: {Recipient}", notification.Recipient);
			return false;
		}
	}
}
