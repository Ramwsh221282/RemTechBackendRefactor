using Mailers.Application.Configs;
using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using Microsoft.Extensions.Options;
using RemTech.Aes.Encryption;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record ChangeMailerSmtpPasswordFunctionArguments(
    Guid Id,
    string NextPassword,
    NpgSqlSession Session)
    : IFunctionArgument;

public sealed class
    ChangeMailerSmtpPasswordFunction : IAsyncFunction<ChangeMailerSmtpPasswordFunctionArguments, Result<Mailer>>
{
    private readonly IOptions<MailersEncryptOptions> _options;
    private readonly IFunction<CreateSmtpPasswordArgument, Result<SmtpPassword>> _createPassword;

    public ChangeMailerSmtpPasswordFunction(
        IOptions<MailersEncryptOptions> options,
        IFunction<CreateSmtpPasswordArgument, Result<SmtpPassword>> createPassword)
    {
        _options = options;
        _createPassword = createPassword;
    }

    public async Task<Result<Mailer>> Invoke(ChangeMailerSmtpPasswordFunctionArguments argument, CancellationToken ct)
    {
        string key = _options.Value.Key;
        if (string.IsNullOrWhiteSpace(key)) return Error.Application("Не сгенерирован ключ шифрования SMTP паролей.");
        CreateSmtpPasswordArgument passArg = new(argument.NextPassword);
        Result<SmtpPassword> passResult = _createPassword.Invoke(passArg);
        if (passResult.IsFailure) return passResult.Error;
        string encryptedSmtpPassword = await new AesEncryption(key).EncryptAsync(passResult.Value.Value);
        QueryMailerArguments query = new(Id: argument.Id);
        await argument.Session.GetTransaction(ct);
        Optional<Mailer> queried = await query.Get(argument.Session, ct, withLock: true);
        if (queried.NoValue) return Error.NotFound("Почтовый отправитель не существует.");
        SmtpPassword encrypted = new(encryptedSmtpPassword);
        MailerMetadata withEncryptedPassword = queried.Value.Metadata with { Password = encrypted };
        Mailer updated = queried.Value with { Metadata = withEncryptedPassword };
        await updated.Update(argument.Session, ct);
        if (!await argument.Session.Commited(ct)) return Error.Application("Не удается зафиксировать изменения.");
        return updated;
    }
}