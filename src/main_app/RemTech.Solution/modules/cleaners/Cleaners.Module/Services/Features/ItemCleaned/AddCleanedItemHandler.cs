using Cleaners.Module.Contracts.ItemCleaned;
using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ItemCleaned;

internal sealed class AddCleanedItemHandler(
    NpgsqlConnection connection,
    Serilog.ILogger logger,
    ItemCleanedMessagePublisher publisher
) : ICommandHandler<AddCleanedItem>
{
    public async Task Handle(AddCleanedItem command, CancellationToken ct = default)
    {
        logger.Information("Adding cleaned item");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        ICleaner withCleanedItem = cleaner.CleanItem();
        ICleaner logged = await withCleanedItem
            .ProduceOutput()
            .PrintTo(new LoggingCleanerVeil(logger))
            .BehaveAsync(ct);
        await logged
            .ProduceOutput()
            .PrintTo(new NpgSqlSavingCleanerVeil(connection))
            .BehaveAsync(ct);
        await publisher.Send(command.Id, ct);
    }
}
