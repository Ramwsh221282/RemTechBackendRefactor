using Cleaners.Module.Cache;
using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Cleaners.Module.Domain.Exceptions;
using Cleaners.Module.RabbitMq;
using Npgsql;
using RabbitMQ.Client;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.PermantlyEnable;

internal sealed class PermantlyEnableCleanerHandler(
    NpgsqlConnection connection,
    Serilog.ILogger logger,
    ConnectionFactory rabbitFactory,
    ICleanerCache cache
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
        CleanerItemsSource itemsSource = new CleanerItemsSource(connection);
        CleanerItemsCollection items = await itemsSource.FetchByTreshold(cleaner, ct);
        if (!items.HasAnything())
            throw new CleanerHasNoItemsToCleanException();
        cleaner = cleaner.StartWork();
        cleaner = await cleaner
            .ProduceOutput()
            .PrintTo(new NpgSqlSavingCleanerVeil(connection))
            .BehaveAsync(ct);
        await cache.Invalidate(cleaner);
        StartCleaningMessage message = new([]);
        items.PrintTo(message);
        StartCleaningPublisher publisher = new(rabbitFactory, logger);
        await publisher.Publish(message, ct);
        return cleaner;
    }
}
