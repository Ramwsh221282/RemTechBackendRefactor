using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.FinishJob;

internal sealed class FinishJobHandler(Serilog.ILogger logger, NpgsqlConnection connection)
    : ICommandHandler<FinishJobCommand>
{
    public async Task Handle(FinishJobCommand command, CancellationToken ct = default)
    {
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        ICleaner finished = cleaner.StopWork(command.Seconds);
        ICleaner logged = await finished
            .ProduceOutput()
            .PrintTo(new LoggingCleanerVeil(logger))
            .BehaveAsync(ct);
        await logged
            .ProduceOutput()
            .PrintTo(new NpgSqlSavingCleanerVeil(connection))
            .BehaveAsync(ct);
    }
}
