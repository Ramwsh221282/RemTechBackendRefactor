using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessObjects;

public sealed class PostgreSqlConnection(
    NpgsqlDataSource dataSource,
    NpgsqlConnection connection,
    bool opened
) : IDbConnection
{
    private NpgsqlTransaction? _transaction;

    public void Dispose()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }

        connection.Dispose();
        dataSource.Dispose();
    }

    public IDbConnection AccessConnection() => connection;

    public IDbTransaction AccessTransaction() => BeginTransaction();

    public async ValueTask<IDbTransaction> BeginTransactionAsync() =>
        _transaction ??= await connection.BeginTransactionAsync();

    public async ValueTask<IDbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel) =>
        _transaction ??= await connection.BeginTransactionAsync(isolationLevel);

    public async ValueTask<IDbTransaction> BeginTransactionAsync(CancellationToken ct) =>
        _transaction ??= await connection.BeginTransactionAsync(ct);

    public async ValueTask<IDbTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken ct
    ) => _transaction ??= await connection.BeginTransactionAsync(isolationLevel, ct);

    public IDbTransaction BeginTransaction() => _transaction ??= connection.BeginTransaction();

    public IDbTransaction BeginTransaction(IsolationLevel il) =>
        _transaction ??= connection.BeginTransaction(il);

    public void ChangeDatabase(string databaseName) => connection.ChangeDatabase(databaseName);

    public void Close() => connection.Close();

    public IDbCommand CreateCommand() => connection.CreateCommand();

    public void Open()
    {
        if (!opened)
            connection.Open();
    }

    [AllowNull]
    public string ConnectionString { get; set; } = connection.ConnectionString;

    public int ConnectionTimeout { get; } = connection.ConnectionTimeout;

    public string Database { get; } = connection.Database;

    public ConnectionState State { get; } = connection.State;

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await dataSource.DisposeAsync();
        await connection.DisposeAsync();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);
        await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);
        await _transaction.RollbackAsync(ct);
    }

    public void Commit()
    {
        throw new NotImplementedException();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }

    public IDbConnection Connection => connection;
    public IsolationLevel IsolationLevel =>
        _transaction?.IsolationLevel ?? IsolationLevel.Unspecified;
}
