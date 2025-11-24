using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed record NpgSqlSession(NpgSqlConnectionFactory Factory) : IAsyncDisposable, IDisposable
{
    private NpgsqlConnection? _connection;
    public NpgsqlTransaction? Transaction { get; private set; }

    public async Task<NpgsqlConnection> GetConnection(CancellationToken ct) =>
        _connection??=await Factory.Create(ct);
    
    public async Task<NpgsqlTransaction> GetTransaction(CancellationToken ct)
    {
        NpgsqlConnection connection = await GetConnection(ct);
        Transaction??= await connection.BeginTransactionAsync(ct);
        return Transaction;
    }

    public async Task<int> WithAffectedCallback(CommandDefinition command, CancellationToken ct)
    {
        NpgsqlConnection connection = await GetConnection(ct);
        int result = await connection.ExecuteAsync(command);
        return result;
    }

    public async Task Execute(CommandDefinition command)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        await connection.ExecuteAsync(command);
    }
    
    public async Task Execute(DynamicParameters parameters, string sql, CancellationToken ct)
    {
        NpgsqlTransaction transaction = await GetTransaction(ct);
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: transaction);
        await connection.ExecuteAsync(command);
    }

    public async Task<int> CountAffected(CommandDefinition command)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        return await connection.ExecuteAsync(command);   
    }

    public async Task<T> QuerySingleRow<T>(CommandDefinition command)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        T result = await connection.QuerySingleAsync<T>(command);
        return result;
    }
    
    public async Task<T?> QueryMaybeRow<T>(CommandDefinition command)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        T? result = await connection.QueryFirstOrDefaultAsync<T>(command);
        return result;
    }

    public async Task<IEnumerable<T>> QueryMultipleRows<T>(CommandDefinition command)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        IEnumerable<T> result = await connection.QueryAsync<T>(command);
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (Transaction != null) await Transaction.DisposeAsync();
    }
    
    public async Task<Result<Unit>> ExecuteUnderTransaction(Func<Task> fn, CancellationToken ct)
    {
        await GetTransaction(ct);
        await fn();
        bool success = await Commited(ct);
        return success ? Unit.Value : Error.Application("Не удается зафиксировать транзакцию.");
    }
    
    public async Task<Result<T>> ExecuteUnderTransaction<T>(Func<Task<Result<T>>> fn, CancellationToken ct)
    {
        await GetTransaction(ct);
        Result<T> result = await fn();
        if (result.IsFailure) return result;
        bool success = await Commited(ct);
        return success ? result : Error.Application("Не удается зафиксировать транзакцию.");
    }
    
    public async Task<Result<T>> ExecuteUnderTransaction<T>(Func<Task<T>> fn, CancellationToken ct)
    {
        await GetTransaction(ct);
        T result = await fn();
        bool success = await Commited(ct);
        return success ? result : Error.Application("Не удается зафиксировать транзакцию.");
    }

    public async Task UnsafeCommit(CancellationToken ct)
    {
        if (Transaction == null) return;
        
        try
        {
            await Transaction.CommitAsync(ct);
        }
        catch
        {
            await Transaction.RollbackAsync(ct);
            throw;
        }
    }
    
    public async Task<bool> Commited(CancellationToken ct)
    {
        if (Transaction == null) return true;
        
        try
        {
            await Transaction.CommitAsync(ct);
            return true;
        }
        catch
        {
            await Transaction.RollbackAsync(ct);
            return false;
        }
    }
    
    public void Dispose()
    {
        _connection?.Dispose();
        Transaction?.Dispose();
    }
}