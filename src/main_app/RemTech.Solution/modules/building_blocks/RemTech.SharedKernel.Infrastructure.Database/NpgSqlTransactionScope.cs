using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlTransactionScope : ITransactionScope
{
    private NpgsqlTransaction Transaction { get; }

    public NpgSqlTransactionScope(NpgsqlTransaction transaction)
    {
        Transaction = transaction;
    }

    public async Task<Result> Commit(CancellationToken ct = default)
    {
        try
        {
            await Transaction.CommitAsync(ct);
            return Result.Success();
        }
        catch
        {
            await Transaction.RollbackAsync(ct);
            return Result.Failure(Error.Application("Ошибка транзакции."));
        }
    }

    public void Dispose()
    {
        Transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
    }
}