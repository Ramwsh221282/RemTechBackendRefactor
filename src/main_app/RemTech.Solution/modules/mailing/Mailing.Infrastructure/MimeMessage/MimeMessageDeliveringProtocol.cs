using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Infrastructure.MimeMessage;

public sealed record MimeMessageDeliveringProtocol : MessageDeliveryProtocol
{
    private const int Port = 587;
    
    public async Task<DeliveredMessage> Process(Mailer mailer, InboxMessage message, CancellationToken ct)
    {
        string senderEmail = mailer.Domain.Email.Value;
        string senderSmtp = mailer.Config.SmtpPassword;
        BodyBuilder builder = new() { TextBody = message.Body.Value };
        MimeKit.MimeMessage mimeMessage = new();
        mimeMessage.Subject = message.Subject.Value;
        mimeMessage.Body = builder.ToMessageBody();
        mimeMessage.To.Add(MailboxAddress.Parse(message.TargetEmail.Value));
        mimeMessage.From.Add(MailboxAddress.Parse(senderEmail));
        using SmtpClient client = new();
        try
        {
            client.AuthenticationMechanisms.Add("XOAUTH2");
            await client.ConnectAsync(mailer.Domain.SmtpHost, Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(senderEmail, mailer.Config.SmtpPassword, ct);
            await client.SendAsync(mimeMessage, ct);
            await client.DisconnectAsync(true, ct);
            MailerDomain withIncreasedSend = mailer.Domain.WithIncreasedSend();
            return new DeliveredMessage(mailer with { Domain = withIncreasedSend } , message);
        }
        catch
        {
            throw ErrorException.Internal("Произошла ошибка при отправке сообщения через SMTP сервис.");
        }
    }
}