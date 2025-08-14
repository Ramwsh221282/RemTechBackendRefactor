using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.DisableCleaner;

internal sealed class DisableCleanerHandler(NpgsqlConnection connection, Serilog.ILogger logger)
    : ICommandHandler<DisableCleanerCommand, ICleaner>
{
    public async Task<ICleaner> Handle(
        DisableCleanerCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("Disabling cleaner");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        cleaner = cleaner.StopWork();
        return cleaner;
    }
}
