using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.DisableCleaner;

internal sealed class DisableCleanerHandler(ICleaners cleaners, Serilog.ILogger logger)
    : ICommandHandler<DisableCleanerCommand, ICleaner>
{
    public async Task<ICleaner> Handle(
        DisableCleanerCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("Disabling cleaner");
        ICleaner cleaner = await cleaners.Single(ct);
        cleaner = cleaner.StopWork();
        return cleaner;
    }
}
