using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed class NpgSqlTransactionScope(NpgSqlSession session) : ITransactionScope
{
    public async Task<Result> Commit(CancellationToken ct = default)
    {
        if (!await session.Commited(ct))
            return Result.Failure(Error.Conflict("Ошибка транзакции."));
        return Result.Success();
    }
}