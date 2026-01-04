using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlSession(NpgSqlConnectionFactory connectionFactory) : IDisposable, IAsyncDisposable
{
    private NpgsqlConnection? _connection;
    public NpgsqlTransaction? Transaction { get; private set; }

    public async Task<NpgsqlConnection> GetConnection(CancellationToken ct)
    {
        if (_connection is not null) return _connection;
        _connection = await connectionFactory.Create(ct);
        return _connection;
    }
    
    public async Task<NpgsqlTransaction> GetTransaction(CancellationToken ct)
    {
        if (Transaction is not null) return Transaction;
        NpgsqlConnection connection = await GetConnection(ct);
        Transaction = await connection.BeginTransactionAsync(ct);
        return Transaction;
    }

    public CommandDefinition FormCommand(string sql, object parameters, CancellationToken ct)
    {
        return new CommandDefinition(sql, parameters, cancellationToken: ct, transaction: Transaction);
    }
    
    public CommandDefinition FormCommand(string sql, DynamicParameters parameters, CancellationToken ct)
    {
        return new CommandDefinition(sql, parameters, cancellationToken: ct, transaction: Transaction);
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
        if (Transaction != null) await Transaction.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }

    public async Task<T?> QuerySingleUsingReader<T>(
        CommandDefinition command, 
        Func<IDataReader, T> mapper) 
        where T : notnull
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<T> result = [];
        while (await reader.ReadAsync())
            result.Add(mapper(reader));
        return result.Count == 0 ? default : result[0];
    }

    public async Task<(T? mainEntity, List<U> relatedEntities)> QuerySingleUsingReader<T, U>(
        CommandDefinition command,
        Func<IDataReader, T> mainEntityMapper,
        Func<IDataReader, U?> relatedEntityMapper,
        IEqualityComparer<T> comparer)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        Dictionary<T, List<U>> mappings = new(comparer);
        while (await reader.ReadAsync())
        {
            T mainEntity = mainEntityMapper(reader);
            
            if (!mappings.ContainsKey(mainEntity))
                mappings.Add(mainEntity, []);
            
            U? related = relatedEntityMapper(reader);
            if (related != null)
                mappings[mainEntity].Add(related);
        }
        return mappings.Count == 0 ? default : (mappings.First().Key, mappings[mappings.First().Key]);
    }

    public async Task<T[]> QueryMultipleUsingReader<T>(CommandDefinition command, Func<IDataReader, T> mapper) where T : notnull
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        List<T> result = [];
        while (await reader.ReadAsync())
            result.Add(mapper(reader));
        return result.ToArray();
    }
    
    public async Task<T[]> QueryMultipleUsingReader<T>(
        CommandDefinition command, 
        Func<IDataReader, T> mapper, 
        IEqualityComparer<T> comparer,
        CancellationToken ct = default)
    {
        HashSet<T> results = new HashSet<T>(comparer);
        NpgsqlConnection connection = await GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        while (await reader.ReadAsync(ct))
        {
            T item = mapper(reader);
            results.Add(item);
        }
        
        return results.ToArray();
    }

    public async Task ExecuteBulk(string sql, object[] parameters)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> ExecuteBulkWithAffectedCount(string sql, object[] parameters)
    {
        NpgsqlConnection connection = await GetConnection(CancellationToken.None);
        return await connection.ExecuteAsync(sql, parameters);
    }
    
    public void Dispose()
    {
        if (Transaction != null)
        {
            Transaction.Dispose();
        }

        if (_connection != null)
        {
            _connection.Dispose();
        }
    }
}