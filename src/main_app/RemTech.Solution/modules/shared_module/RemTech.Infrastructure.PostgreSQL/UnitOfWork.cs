using Microsoft.EntityFrameworkCore;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Database;

namespace RemTech.Infrastructure.PostgreSQL;

public sealed class UnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly Serilog.ILogger _logger;

    public UnitOfWork(TContext dbContext, Serilog.ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result.Pattern.Result> SaveChanges(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return Result.Pattern.Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Fatal("Exception at saving changes: {ex}", ex);
            return Result.Pattern.Result.Failure(
                new Error("Ошибка при сохранении данных.", ErrorCodes.Internal)
            );
        }
    }

    public void Dispose() => _dbContext.Dispose();

    public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();
}
