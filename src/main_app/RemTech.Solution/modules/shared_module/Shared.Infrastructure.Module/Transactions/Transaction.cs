using System.Data;
using Npgsql;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace Shared.Infrastructure.Module.Transactions;

public sealed class Transaction : ITransaction
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;

    public Transaction(NpgsqlConnection connection) => _connection = connection;

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }

    public void Interact(Action<IDbConnection> action) => action(_connection);

    public void Interact(Action<IDbTransaction?> action) => action(_transaction);

    public Task<T> Interact<T>(Func<IDbConnection, Task<T>> action) => action(_connection);

    public async Task WithTransaction(CancellationToken ct = default) =>
        _transaction ??= await _connection.BeginTransactionAsync(ct);

    public async Task<Status> Commit(CancellationToken ct = default)
    {
        try
        {
            if (_transaction == null)
                return Status.Success();
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
}
