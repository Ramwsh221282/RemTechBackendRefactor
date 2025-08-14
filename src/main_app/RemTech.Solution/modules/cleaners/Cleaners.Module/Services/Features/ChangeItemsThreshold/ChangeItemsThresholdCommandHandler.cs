using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeItemsThreshold;

internal sealed class ChangeItemsThresholdCommandHandler(
    NpgsqlConnection connection,
    Serilog.ILogger logger
) : ICommandHandler<ChangeItemsThresholdCommand, ICleaner>
{
    public async Task<ICleaner> Handle(
        ChangeItemsThresholdCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("Change items threshold started");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        cleaner = cleaner.ChangeItemsToCleanThreshold(command.Threshold);
        return cleaner;
    }
}
