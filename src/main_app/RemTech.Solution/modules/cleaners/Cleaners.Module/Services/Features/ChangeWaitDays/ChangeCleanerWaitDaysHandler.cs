using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeWaitDays;

internal sealed class ChangeCleanerWaitDaysHandler(
    NpgsqlConnection connection,
    Serilog.ILogger logger
) : ICommandHandler<ChangeCleanerWaitDays, ICleaner>
{
    public async Task<ICleaner> Handle(
        ChangeCleanerWaitDays command,
        CancellationToken ct = default
    )
    {
        logger.Information("Changing cleaner wait days");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        return cleaner.ChangeWaitDays(command.Days);
    }
}
