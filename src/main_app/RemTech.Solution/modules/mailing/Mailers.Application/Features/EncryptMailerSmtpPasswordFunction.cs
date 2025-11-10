using Mailers.Application.Configs;
using Mailers.Core.MailersContext;
using Microsoft.Extensions.Options;
using RemTech.Aes.Encryption;

namespace Mailers.Application.Features;

public sealed record EncryptMailerSmtpPasswordFunctionArgument(Mailer Mailer) : IFunctionArgument;

public sealed class EncryptMailerSmtpPasswordFunction : IAsyncFunction<EncryptMailerSmtpPasswordFunctionArgument, Result<Mailer>>
{
    private readonly MailersEncryptOptions _options;

    public EncryptMailerSmtpPasswordFunction(IOptions<MailersEncryptOptions> options) =>
        _options = options.Value;

    public async Task<Result<Mailer>> Invoke(EncryptMailerSmtpPasswordFunctionArgument argument, CancellationToken ct)
    {
        string key = _options.Key;
        string password = argument.Mailer.Metadata.Password.Value;

        if (string.IsNullOrWhiteSpace(key))
            return Error.Application("Ключ для шифрования пароля почтового отправителя не сформирован.");

        if (string.IsNullOrWhiteSpace(password))
            return Error.Validation("Пароль почтового отправителя для шифрования не задан.");

        AesEncryption encryption = new(key);
        string encryptedPassword = await encryption.EncryptAsync(password);
        SmtpPassword newPassword = new SmtpPassword(encryptedPassword);

        MailerMetadata newMetadata = argument.Mailer.Metadata with { Password = newPassword };
        return argument.Mailer with { Metadata = newMetadata };
    }
}