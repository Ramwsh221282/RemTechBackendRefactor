using Dapper;
using Npgsql;

namespace RemTech.NpgSql.Abstractions;

public sealed record NpgSqlSession(NpgSqlConnectionFactory Factory) : IAsyncDisposable, IDisposable
{
    private NpgsqlConnection? _connection;
    public NpgsqlTransaction? Transaction { get; private set; }

    public async Task<NpgsqlConnection> GetConnection(CancellationToken ct) =>
        _connection??=await Factory.Create(ct);
    
    public async Task<NpgsqlTransaction> GetTransaction(CancellationToken ct)
    {
        var connection = await GetConnection(ct);
        Transaction??= await connection.BeginTransactionAsync(ct);
        return Transaction;
    }

    public async Task Execute(CommandDefinition command)
    {
        var connection = await GetConnection(CancellationToken.None);
        await connection.ExecuteAsync(command);
    }

    public async Task<int> CountAffected(CommandDefinition command)
    {
        var connection = await GetConnection(CancellationToken.None);
        return await connection.ExecuteAsync(command);   
    }

    public async Task<T> QuerySingleRow<T>(CommandDefinition command)
    {
        var connection = await GetConnection(CancellationToken.None);
        var result = await connection.QuerySingleAsync<T>(command);
        return result;
    }
    
    public async Task<T?> QueryMaybeRow<T>(CommandDefinition command)
    {
        var connection = await GetConnection(CancellationToken.None);
        var result = await connection.QueryFirstOrDefaultAsync<T>(command);
        return result;
    }

    public async Task<IEnumerable<T>> QueryMultipleRows<T>(CommandDefinition command)
    {
        var connection = await GetConnection(CancellationToken.None);
        var result = await connection.QueryAsync<T>(command);
        return result;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (Transaction != null) await Transaction.DisposeAsync();
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