using Microsoft.EntityFrameworkCore.Storage;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Database;

namespace RemTech.Infrastructure.PostgreSQL;

public sealed class TransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;
    private readonly Serilog.ILogger _logger;

    public TransactionScope(IDbContextTransaction transaction, Serilog.ILogger logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public void Dispose() => _transaction.Dispose();

    public async ValueTask DisposeAsync() => await _transaction.DisposeAsync();

    public async Task<Result.Pattern.Result> Commit(CancellationToken ct = default)
    {
        try
        {
            await _transaction.CommitAsync(ct);
            return Result.Pattern.Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Fatal("Transaction failed: {Ex}", ex);
            await _transaction.RollbackAsync(ct);
            Error error = new Error("Ошибка во время транзакции.", ErrorCodes.Validation);
            return Result.Pattern.Result.Failure(error);
        }
    }
}
