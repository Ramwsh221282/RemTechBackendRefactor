using Dapper;
using Npgsql;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgTransactionalParserJournal : IDisposable, IAsyncDisposable
{
    private readonly Queue<CommandDefinition> _queue;
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction _transaction;

    public PgTransactionalParserJournal(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
        _queue = [];
    }

    public void AddOperation(CommandDefinition command) => _queue.Enqueue(command);

    public async Task<Status> Process(CancellationToken ct)
    {
        while (_queue.TryDequeue(out CommandDefinition command))
        {
            try
            {
                await _connection.ExecuteAsync(command);
            }
            catch
            {
                return new Error("Ошибка транзакции.", ErrorCodes.Conflict);
            }
        }

        try
        {
            await _transaction.CommitAsync(ct);
            return Status.Success();
        }
        catch
        {
            await _transaction.RollbackAsync(ct);
            return new Error("Ошибка транзакции.", ErrorCodes.Conflict);
        }
    }

    public NpgsqlTransaction Transaction() => _transaction;

    public void Dispose()
    {
        _connection.Dispose();
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _transaction.DisposeAsync();
    }
}
