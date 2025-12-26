using Mailers.Application.Configs;
using Mailers.Application.Features.Encryptions;
using Mailers.Core.EmailsModule;
using Mailers.Core.MailersModule;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Mailers.Application.Features.MailerSmtpProviding;

public static class MailerSmtpProviderModule
{
    extension(Mailer mailer)
    {
        public async Task<Result<MailerSending>> SendMessage(
            IOptions<MailersEncryptOptions> encrypt,
            MailerDeliveryArgs deliveryArgs, 
            CancellationToken ct)
        {
            Result<MailerSending> sending = mailer.SendEmail(deliveryArgs);
            if (sending.IsFailure) return sending.Error;
            
            Result<MimeMessage> message = sending.Value.FormMessage(deliveryArgs);
            if (message.IsFailure) return message.Error;
            
            Result<SmtpHostRegistry> registry = mailer.DispatchHost();
            if (registry.IsFailure) return registry.Error;

            Result<Unit> result = await sending.Value.Sent(encrypt, registry, message, ct);
            return result.IsFailure ? result.Error : sending;
        }

        private Result<SmtpHostRegistry> DispatchHost()
        {
            const int port = 587;
            Email email = mailer.Metadata.Email;
            Optional<string> hostUrl = Mailer.DispatchHostUrlString(email);
            
            if (hostUrl.NoValue) return Conflict($"Не удается разрешить сервис почты у {email.Value}");
            return new SmtpHostRegistry(hostUrl.Value, port);
        }

        private static Optional<string> DispatchHostUrlString(Email email)
        {
            string emailString = email.Value;
            string domain = emailString.Split('@')[^1].Split('.')[0];
            
            return domain switch
            {
                "gmail" => Optional.Some("smtp.gmail.com"),
                "mail" => Optional.Some("smtp.mail.ru"),
                "yandex" => Optional.Some("smtp.yandex.ru"),
                _ => Optional.None<string>()
            };
        }
    }

    extension(MailerSending sending)
    {
        private Result<MimeMessage> FormMessage(MailerDeliveryArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Subject)) return Error.Validation("Тема письма не указана.");
            if (string.IsNullOrWhiteSpace(args.Body)) return Error.Validation("Тело письма не указано.");
            
            BodyBuilder builder = new() { TextBody = args.Body };
            MimeMessage message = new();
            
            message.Subject = args.Subject;
            message.To.Add(MailboxAddress.Parse(sending.Mailer.Metadata.Email.Value));
            message.From.Add(MailboxAddress.Parse(sending.Message.DeliveryInfo.To.Value));
            message.Body = builder.ToMessageBody();
            
            return message;
        }

        private async Task<Result<Unit>> Sent(
            IOptions<MailersEncryptOptions> encrypt,
            SmtpHostRegistry registry, 
            MimeMessage message, 
            CancellationToken ct)
        {
            string fromEmail = sending.Mailer.Metadata.Email.Value;
            string fromSmtp = await encrypt.Decrypted(sending.Mailer);
            
            using SmtpClient client = new();
            try
            {
                client.AuthenticationMechanisms.Add("XOAUTH2");
                await client.ConnectAsync(registry.Host, registry.Port, SecureSocketOptions.StartTls, ct);
                await client.AuthenticateAsync(fromEmail, fromSmtp, ct);
                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);
                return Unit.Value;
            }
            catch
            {
                return Error.Application("Не удается отправить письмо.");
            }
        }
    }
}