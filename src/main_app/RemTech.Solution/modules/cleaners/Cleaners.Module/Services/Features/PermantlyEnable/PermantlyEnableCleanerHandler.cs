using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.PermantlyEnable;

internal sealed class PermantlyEnableCleanerHandler(
    NpgsqlConnection connection,
    Serilog.ILogger logger
) : ICommandHandler<PermantlyEnableCleanerCommand, ICleaner>
{
    public async Task<ICleaner> Handle(
        PermantlyEnableCleanerCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("Permantly enabling cleaner.");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        cleaner = cleaner.StartWork();
        return cleaner;
    }
}
