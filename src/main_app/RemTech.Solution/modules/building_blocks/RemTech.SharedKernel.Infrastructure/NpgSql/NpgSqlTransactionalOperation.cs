using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed class NpgSqlTransactionalOperation(NpgSqlSession session) : ITransactionalOperation
{
    public async Task<Result<U>> Execute<U>(AsyncOperation<U> operation, CancellationToken ct = default)
    where U : notnull
    {
        try
        {
            Result<U> operationResult = await operation.Process();
            if (operationResult.IsFailure)
                return operationResult.Error;
            await session.UnsafeCommit(ct);
            return operationResult.Value;
        }
        catch
        {
            return Result.Failure<U>(Error.Application("Не удается зафиксировать транзакцию"));
        }
    }
}