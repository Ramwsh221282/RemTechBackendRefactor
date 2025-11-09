using Mailers.Core.MailersContext;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record RegisterMailerInApplicationFunctionArgument(
    string Email, 
    string SmtpPassword, 
    NpgSqlSession Session)
    : IFunctionArgument;

public sealed class RegisterMailerInApplicationFunction : IAsyncFunction<RegisterMailerInApplicationFunctionArgument, Result<Mailer>>
{
    private readonly IFunction<CreateMailerFunctionArgument, Result<Mailer>> _createMailer;
    private readonly IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>> _insertMailer;
    
    public RegisterMailerInApplicationFunction(
        IFunction<CreateMailerFunctionArgument, Result<Mailer>> createMailer,
        IAsyncFunction<InsertMailerFunctionArgument, Result<Unit>> insertMailer)
    {
        _createMailer = createMailer;
        _insertMailer = insertMailer;
    }
    
    public async Task<Result<Mailer>> Invoke(RegisterMailerInApplicationFunctionArgument argument, CancellationToken ct)
    {
        var createMeta = new CreateMailerMetadataArguments(argument.Email, argument.SmtpPassword);
        var createStats = new CreateMailerStatisticsFunctionArgument();
        var mailer = _createMailer.Invoke(new CreateMailerFunctionArgument(createMeta, createStats));
        if (mailer.IsFailure) return mailer.Error;
        var insert = await _insertMailer.Invoke(new InsertMailerFunctionArgument(argument.Session, mailer), ct);
        if (insert.IsFailure) return insert.Error;
        return mailer.Value;
    }
}