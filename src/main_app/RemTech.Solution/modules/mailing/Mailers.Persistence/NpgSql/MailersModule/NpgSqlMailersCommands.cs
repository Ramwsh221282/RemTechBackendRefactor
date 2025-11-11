using Mailers.Core.EmailsModule;
using Mailers.Core.MailersModule;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql.MailersModule;

public sealed record NpgSqlMailersCommands(
    NpgSqlSession Session,
    InsertMailer Insert,
    DeleteMailer Delete,
    UpdateMailer Update,
    HasUniqueMailerEmail HasUniqueEmail,
    InsertMailerSending InsertSendings,
    GetMailer Get,
    
    GetManyMailers GetMany) : IDisposable, IAsyncDisposable
{
    public void Dispose() =>
        Session.Dispose();

    public async ValueTask DisposeAsync() =>
        await Session.DisposeAsync();

    public async Task<Result<Unit>> ExecuteInsert(Mailer mailer, CancellationToken ct) =>
        await Insert(mailer, ct);
    
    public async Task<Result<Unit>> ExecuteDelete(Mailer mailer, CancellationToken ct) =>
        await Delete(mailer, ct);
    
    public async Task<Result<Unit>> ExecuteUpdate(Mailer mailer, CancellationToken ct) =>
        await Update(mailer, ct);
    
    public async Task<bool> IsEmailUnique(Mailer mailer, CancellationToken ct) =>
        await HasUniqueEmail(mailer.Metadata.Email, ct);
    
    public async Task<bool> IsEmailUnique(Email email, CancellationToken ct) =>
        await HasUniqueEmail(email, ct);
    
    public async Task<Optional<Mailer>> GetMailer(QueryMailerArguments args, CancellationToken ct) =>
        await Get(args, ct);
    
    public async Task<IEnumerable<Mailer>> GetMailers(QueryMailerArguments args, CancellationToken ct) =>
        await GetMany(args, ct);
    
    public async Task<Result<Unit>> InsertMailerSending(MailerSending sending, CancellationToken ct) =>
        await InsertSendings(sending, ct);
    
    public async Task<Result<Unit>> ExecuteUnderTransaction(Func<NpgSqlMailersCommands, Task> fn, CancellationToken ct)
    {
        await Session.GetTransaction(ct);
        await fn(this);
        bool success = await Session.Commited(ct);
        return success ? Unit.Value : Application("Не удается зафиксировать транзакцию.");
    }
    
    public async Task<Result<T>> ExecuteUnderTransaction<T>(Func<NpgSqlMailersCommands, Task<Result<T>>> fn, CancellationToken ct)
    {
        await Session.GetTransaction(ct);
        Result<T> result = await fn(this);
        if (result.IsFailure) return result;
        bool success = await Session.Commited(ct);
        return success ? result : Application("Не удается зафиксировать транзакцию.");
    }
    
    public async Task<Result<T>> ExecuteUnderTransaction<T>(Func<NpgSqlMailersCommands, Task<T>> fn, CancellationToken ct)
    {
        await Session.GetTransaction(ct);
        T result = await fn(this);
        bool success = await Session.Commited(ct);
        return success ? result : Application("Не удается зафиксировать транзакцию.");
    }
}