using Mailers.Core.MailersContext;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record RegisterMailerInApplicationFunctionArgument(
    string Email,
    string SmtpPassword,
    NpgSqlSession Session)
    : IFunctionArgument;

public sealed class
    RegisterMailerInApplicationFunction : IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>
{
    private readonly IFunction<CreateMailerFunctionArgument, Result<Mailer>> _createMailer;
    private readonly IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>> _insertMailer;
    private readonly IAsyncFunction<EncryptMailerSmtpPasswordFunctionArgument, Result<Mailer>> _encryptMailer;

    public RegisterMailerInApplicationFunction(
        IFunction<CreateMailerFunctionArgument, Result<Mailer>> createMailer,
        IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>> insertMailer,
        IAsyncFunction<EncryptMailerSmtpPasswordFunctionArgument, Result<Mailer>> encryptMailer)
    {
        _createMailer = createMailer;
        _insertMailer = insertMailer;
        _encryptMailer = encryptMailer;
    }

    public async Task<Result<Mailer>> Invoke(RegisterMailerInApplicationFunctionArgument argument, CancellationToken ct)
    {
        CreateMailerMetadataArguments createMeta = new(argument.Email, argument.SmtpPassword);
        CreateMailerStatisticsFunctionArgument createStats = new();
        CreateMailerFunctionArgument createMailerArg = new(createMeta, createStats);
        Result<Mailer> mailer = _createMailer.Invoke(createMailerArg);
        if (mailer.IsFailure) return mailer.Error;
        EncryptMailerSmtpPasswordFunctionArgument encryptMeta = new(mailer);
        Result<Mailer> encrypted = await _encryptMailer.Invoke(encryptMeta, ct);
        if (encrypted.IsFailure) return encrypted.Error;
        InsertMailerFunctionArgument insertMetaArgs = new(argument.Session, encrypted);
        Result<Unit> insert = await _insertMailer.Invoke(insertMetaArgs, ct);
        if (insert.IsFailure) return insert.Error;
        return mailer.Value;
    }
}