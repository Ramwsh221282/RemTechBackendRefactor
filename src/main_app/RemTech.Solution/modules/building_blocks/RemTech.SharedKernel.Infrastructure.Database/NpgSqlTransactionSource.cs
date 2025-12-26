using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlTransactionSource(NpgSqlSession session) : ITransactionSource
{
    public async Task<ITransactionScope> BeginTransaction(CancellationToken ct = default)
    {
        await session.GetTransaction(ct);
        return new NpgSqlTransactionScope(session);
    }
}