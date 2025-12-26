using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlTransactionScope(NpgSqlSession session) : ITransactionScope
{
    public async Task<Result> Commit(CancellationToken ct = default)
    {
        try
        {
            await session.CommitTransaction(ct);
            return Result.Success();
        }
        catch(Exception)
        {
            return Result.Failure(Error.Conflict("Ошибка транзакции."));
        }
    }
}