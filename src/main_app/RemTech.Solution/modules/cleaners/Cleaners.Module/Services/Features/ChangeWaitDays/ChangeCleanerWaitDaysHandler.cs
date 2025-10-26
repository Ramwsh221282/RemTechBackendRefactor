using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeWaitDays;

internal sealed class ChangeCleanerWaitDaysHandler(ICleaners cleaners, Serilog.ILogger logger)
    : ICommandHandler<ChangeCleanerWaitDays, ICleaner>
{
    public async Task<ICleaner> Handle(
        ChangeCleanerWaitDays command,
        CancellationToken ct = default
    )
    {
        logger.Information("Changing cleaner wait days");
        ICleaner cleaner = await cleaners.Single(ct);
        return cleaner.ChangeWaitDays(command.Days);
    }
}
