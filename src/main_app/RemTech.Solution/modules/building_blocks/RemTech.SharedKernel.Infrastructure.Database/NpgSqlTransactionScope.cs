using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlTransactionScope : ITransactionScope
{
    private NpgsqlTransaction Transaction { get; }
    private Serilog.ILogger? Logger;

    public NpgSqlTransactionScope(NpgsqlTransaction transaction, Serilog.ILogger? logger)
    {
        Transaction = transaction;
        Logger = logger?.ForContext<NpgSqlTransactionScope>();
    }

    public async Task<Result> Commit(CancellationToken ct = default)
    {
        try
        {
            Logger?.Information("Committing transaction");
            await Transaction.CommitAsync(ct);
            Logger?.Information("Transaction committed");
            return Result.Success();
        }
        catch(Exception ex)
        {
            Logger?.Error(ex, "Error committing transaction");
            await Transaction.RollbackAsync(ct);
            Logger?.Information("Transaction rolled back");
            return Result.Failure(Error.Conflict("Ошибка транзакции."));
        }
    }

    public void Dispose()
    {
        Transaction.Dispose();
        Logger?.Debug("Transaction scope disposed");
    }

    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
        Logger?.Debug("Transaction scope disposed asynchronously");
    }
}