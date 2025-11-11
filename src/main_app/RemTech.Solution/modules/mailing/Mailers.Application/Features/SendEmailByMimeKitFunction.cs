using Mailers.Application.Configs;
using Mailers.Core.MailersContext;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RemTech.Aes.Encryption;

namespace Mailers.Application.Features;

public sealed record SendEmailByMimeKitFunctionArgument(
    Mailer Mailer,
    Email To,
    string Subject,
    string TextBody,
    string? HtmlBody = null) : IFunctionArgument;

public sealed class SendEmailByMimeKitFunction : IAsyncFunction<SendEmailByMimeKitFunctionArgument, Result<Unit>>
{
    private const int Port = 587;
    private readonly IOptions<MailersEncryptOptions> _encryptOptions;

    public SendEmailByMimeKitFunction(IOptions<MailersEncryptOptions> encryptOptions)
    {
        _encryptOptions = encryptOptions;
    }

    public async Task<Result<Unit>> Invoke(SendEmailByMimeKitFunctionArgument argument, CancellationToken ct)
    {
        string key = _encryptOptions.Value.Key;
        if (string.IsNullOrWhiteSpace(key))
            return Error.Application("Не удается отправить сообщение. Почтовый отправитель не имеет SMTP ключа");

        AesEncryption encryption = new(key);
        string decryption = await encryption.DecryptAsync(argument.Mailer.Metadata.Password.Value);
        if (string.IsNullOrWhiteSpace(decryption))
            return Error.Application("Не удается расшифровать SMTP ключ у почтового отправителя.");

        Result<MailerSmtpServiceName> smtpHost = argument.Mailer.ResolveSmtpService();
        if (smtpHost.IsFailure)
            return smtpHost.Error;

        MimeEntity messageBody = BuildMessageBody(argument);
        MimeMessage message = BuildMessage(argument);
        message.Body = messageBody;

        using MailKit.Net.Smtp.SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(smtpHost.Value.Name, Port, false, ct);
        await smtpClient.AuthenticateAsync(argument.Mailer.Metadata.Email.Value, decryption, ct);
        await smtpClient.SendAsync(message, ct);
        await smtpClient.DisconnectAsync(true, ct);
        return Unit.Value;
    }

    private static MimeMessage BuildMessage(SendEmailByMimeKitFunctionArgument argument)
    {
        MimeMessage message = new();
        string from = argument.Mailer.Metadata.Email.Value;
        string to = argument.To.Value;
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = argument.Subject;
        return message;
    }

    private static MimeEntity BuildMessageBody(SendEmailByMimeKitFunctionArgument argument)
    {
        BodyBuilder bodyBuilder = new() { TextBody = argument.TextBody };
        if (argument.HtmlBody != null)
            bodyBuilder.HtmlBody = argument.HtmlBody;
        return bodyBuilder.ToMessageBody();
    }
}