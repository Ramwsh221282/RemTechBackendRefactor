using Npgsql;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlTransactionSource(NpgSqlSession session, Serilog.ILogger? logger) : ITransactionSource
{
    public async Task<ITransactionScope> BeginTransaction(CancellationToken ct = default)
    {
        NpgsqlTransaction transaction = await session.GetTransaction(ct);
        return new NpgSqlTransactionScope(transaction, logger);
    }
}