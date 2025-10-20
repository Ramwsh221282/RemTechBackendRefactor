using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.Enable;

internal sealed class EnableCleanerHandler(NpgsqlConnection connection, Serilog.ILogger logger)
    : ICommandHandler<EnableCleaner, ICleaner>
{
    public async Task<ICleaner> Handle(EnableCleaner command, CancellationToken ct = default)
    {
        logger.Information("Enabling cleaner");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        return cleaner.StartWait();
    }
}
