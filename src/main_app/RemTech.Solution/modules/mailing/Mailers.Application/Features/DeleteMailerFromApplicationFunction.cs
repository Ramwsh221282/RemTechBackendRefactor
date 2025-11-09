using Mailers.Core.MailersContext;
using Mailers.Persistence.NpgSql;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Application.Features;

public sealed record DeleteMailerFromApplicationFunctionArgument(Guid Id, NpgSqlSession Session) : IFunctionArgument;

public sealed class DeleteMailerFromApplicationFunction : IAsyncFunction<DeleteMailerFromApplicationFunctionArgument, Result<Mailer>>
{
    private readonly IAsyncFunction<DeleteMailerFunctionArgument, Result<Unit>> _delete;

    public DeleteMailerFromApplicationFunction(IAsyncFunction<DeleteMailerFunctionArgument, Result<Unit>> delete)
    {
        _delete = delete;
    }
    
    public async Task<Result<Mailer>> Invoke(DeleteMailerFromApplicationFunctionArgument argument, CancellationToken ct)
    {
        var query = new QueryMailerArguments(Id: argument.Id);
        var mailer = await query.Get(argument.Session, ct);
        if (mailer.NoValue) return NotFound("Почтовый отправитель не существует.");
        var deletion = await _delete.Invoke(new DeleteMailerFunctionArgument(mailer.Value, argument.Session), ct);
        if (deletion.IsFailure) return deletion.Error;
        return mailer.Value;
    }
}