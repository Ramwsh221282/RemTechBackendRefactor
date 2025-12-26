using System.Data;
using Npgsql;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;
using Shared.Infrastructure.Module.Postgres;

namespace Shared.Infrastructure.Module.Transactions;

public sealed class TransactionManager(PostgresDatabase database) : ITransactionManager
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }

    public async Task Begin(CancellationToken ct = default)
    {
        if (_connection != null)
            return;
        _connection = await database.DataSource.OpenConnectionAsync(ct);
        _transaction = await _connection.BeginTransactionAsync(ct);
    }

    public async Task<Status> Commit(CancellationToken ct = default)
    {
        try
        {
            if (_transaction == null)
                return Status.Internal("Transaction was not started.");

            await _transaction.CommitAsync(ct);
            return Status.Success();
        }
        catch
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(ct);

            return Status.Internal("Transaction error.");
        }
    }

    public void AccessConnection(Action<IDbConnection> action)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection is not initialized.");
        action(_connection);
    }

    public void AccessConnection(Action<IDbConnection, IDbTransaction> action)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection is not initialized.");
        if (_transaction == null)
            throw new InvalidOperationException("Transaction is not initialized.");
        action(_connection, _transaction);
    }

    public async Task Execute(Func<IDbConnection, Task> func)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection is not initialized.");
        await func(_connection);
    }

    public async Task<T> Execute<T>(Func<IDbConnection, Task<T>> func)
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection is not initialized.");
        return await func(_connection);
    }
}