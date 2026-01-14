using Identity.Domain.Accounts.Features.ConfirmTicket;
using Identity.Domain.Accounts.Models;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Commands.ConfirmTicket;

public sealed class ConfirmTicketCacheInvalidator(Serilog.ILogger logger, HybridCache cache)
    : ICacheInvalidator<ConfirmTicketCommand, Account>
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<ConfirmTicketCacheInvalidator>();
    private HybridCache Cache { get; } = cache;

    public async Task InvalidateCache(
        ConfirmTicketCommand command,
        Account result,
        CancellationToken ct = default
    )
    {
        string key = $"get_user_{result.Id.Value}";
        await Cache.RemoveAsync(key, ct);
        Logger.Information("Invalidated cache for key {Key}", key);
    }
}
