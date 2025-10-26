using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeItemsThreshold;

internal sealed class ChangeItemsThresholdCommandHandler(ICleaners cleaners, Serilog.ILogger logger)
    : ICommandHandler<ChangeItemsThresholdCommand, ICleaner>
{
    public async Task<ICleaner> Handle(
        ChangeItemsThresholdCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("Change items threshold started");
        ICleaner cleaner = await cleaners.Single(ct);
        cleaner = cleaner.ChangeItemsToCleanThreshold(command.Threshold);
        return cleaner;
    }
}
