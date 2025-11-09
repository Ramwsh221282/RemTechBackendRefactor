using Dapper;
using Mailing.Module.MailersContext;
using Mailing.Module.MailersContext.MetadataContext;
using Mailing.Module.MailersContext.StatisticsContext;
using Mailing.Module.Traits;
using Npgsql;
using RemTech.Core.Shared.Result;

namespace Mailing.Module.Infrastructure.NpgSql;

internal sealed record PgPersistence(
    NpgsqlConnection Connection,
    NpgsqlTransaction? Transaction = null)
    :
        IDisposable,
        IAsyncDisposable,
        IPersistenceEngine,
        IAsyncSourceOf<PgMailerByIdSearch, Mailer>
{
    public async ValueTask DisposeAsync()
    {
        if (Transaction != null)
            await Transaction.DisposeAsync();
        await Connection.DisposeAsync();
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection.Dispose();
    }

    public async Task Execute(Func<NpgsqlConnection, Task> action)
    {
        await action(Connection);
    }

    public async Task Execute(CommandDefinition command)
    {
        await Execute(conn => conn.ExecuteAsync(command));
    }

    public async Task<T?> Query<T>(CommandDefinition command)
    {
        var @element = await Query(conn => conn.QueryFirstOrDefaultAsync<T>(command));
        return @element;
    }

    public async Task<T> Query<T>(Func<NpgsqlConnection, Task<T>> func)
    {
        var task = func(Connection);
        return await task;
    }

    public async Task<Status<Mailer>> Find(PgMailerByIdSearch searchEngine, CancellationToken ct)
    {
        var tableMailer = await Query<TableMailer?>(new CommandDefinition(
            """
            SELECT * FROM mailers_module.mailers
            WHERE id = @id
            """, new { id = searchEngine.Id },
            cancellationToken: ct));
        return tableMailer == null
            ? Status<Mailer>.Failure(Error.NotFound("Почтовый отправитель не найден."))
            : Status<Mailer>.Success(tableMailer.Create());
    }
}

internal sealed class TableMailer : ICreatorOf<Mailer>
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required int SendLimit { get; init; }
    public required int SendCurrent { get; init; }

    public Mailer Create()
    {
        var meta = new MailerMetadata(Id, Email, Password);
        var stats = new MailerStatistics(SendLimit, SendCurrent);
        return new Mailer(meta, stats);
    }
}

public sealed record PgMailerByIdSearch(Guid Id) : SearchCriteria;