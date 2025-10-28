using RemTech.Core.Shared.Transactions;
using Shared.Infrastructure.Module.Postgres;

namespace Shared.Infrastructure.Module.Transactions;

public sealed class TransactionManager : ITransactionManager
{
    private readonly PostgresDatabase _database;

    public TransactionManager(PostgresDatabase database) => _database = database;

    public async Task<ITransaction> Create(CancellationToken ct = default)
    {
        var connection = await _database.DataSource.OpenConnectionAsync(ct);
        return new Transaction(connection);
    }
}
