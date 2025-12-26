using Mailing.Core.Inbox;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Core.PrimitivesModule.Immutable;

namespace Mailing.Infrastructure.MimeMessage;

public sealed record MimeMessageDeliveringProtocol : MessageDeliveryProtocol
{
    private const int Port = 587;
    
    public async Task<DeliveredMessage> Process(Mailer mailer, InboxMessage message, CancellationToken ct)
    {
        Mailer validMailer = mailer.Validated();
        ImmutableString smtpHost = new(validMailer.Domain.SmtpHost);
        ImmutableString smtpPassword = new(mailer.Config.SmtpPassword);
        ImmutableString senderEmail = new(mailer.Domain.Email.Value);
        BodyBuilder builder = new() { TextBody = message.Body.Value };
        MimeKit.MimeMessage mimeMessage = new();
        mimeMessage.Subject = message.Subject.Value;
        mimeMessage.Body = builder.ToMessageBody();
        mimeMessage.To.Add(MailboxAddress.Parse(message.TargetEmail.Value));
        mimeMessage.From.Add(MailboxAddress.Parse(senderEmail.Read()));
        using SmtpClient client = new();
        try
        {
            client.AuthenticationMechanisms.Add("XOAUTH2");
            await client.ConnectAsync(smtpHost.Read(), Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(senderEmail.Read(), smtpPassword.Read(), ct);
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