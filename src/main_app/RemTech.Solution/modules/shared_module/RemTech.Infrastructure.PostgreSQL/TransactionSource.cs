using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RemTech.UseCases.Shared.Database;

namespace RemTech.Infrastructure.PostgreSQL;

public sealed class TransactionSource<TContext> : ITransactionSource
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly Serilog.ILogger _logger;

    public TransactionSource(TContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ITransactionScope> BeginTransactionScope(CancellationToken ct = default)
    {
        IDbContextTransaction txn = await _context.Database.BeginTransactionAsync(ct);
        return new TransactionScope(txn, _logger);
    }

    public void Dispose() => _context.Dispose();

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}
