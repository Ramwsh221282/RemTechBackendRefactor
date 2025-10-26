using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.Enable;

internal sealed class EnableCleanerHandler(ICleaners cleaners, Serilog.ILogger logger)
    : ICommandHandler<EnableCleaner, ICleaner>
{
    public async Task<ICleaner> Handle(EnableCleaner command, CancellationToken ct = default)
    {
        logger.Information("Enabling cleaner");
        ICleaner cleaner = await cleaners.Single(ct);
        return cleaner.StartWait();
    }
}
