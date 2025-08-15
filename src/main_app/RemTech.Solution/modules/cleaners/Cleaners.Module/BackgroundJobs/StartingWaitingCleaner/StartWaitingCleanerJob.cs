using Cleaners.Module.Cache;
using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Cleaners.Module.RabbitMq;
using Npgsql;
using Quartz;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Cleaners.Module.BackgroundJobs.StartingWaitingCleaner;

public sealed class StartWaitingCleanerJob(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    ConnectionFactory rabbitMqFactory,
    ConnectionMultiplexer multiplexer
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        ICleaners cleaners = new ReadyToStartCleaners(connection);
        try
        {
            ICleaner cleaner = await cleaners.Single();
            logger.Information("Found cleaner to start.");
            CleanerItemsCollection items = await FetchItemsToClean(cleaner, connection);
            if (!items.HasAnything())
            {
                logger.Information("Cleaner has no items to clean.");
                return;
            }
            ICleaner started = await StartCleaner(cleaner, connection);
            await InvalidateCache(started);
            await PublishToRabbit(items);
        }
        catch (NoCleanersToStartExistException ex)
        {
            logger.Information("{Entrance}. {Ex}.", nameof(StartWaitingCleanerJob), ex.Message);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(StartWaitingCleanerJob), ex.Message);
        }
    }

    private async Task PublishToRabbit(CleanerItemsCollection items)
    {
        StartCleaningMessage message = new StartCleaningMessage([]);
        items.PrintTo(message);
        StartCleaningPublisher publisher = new StartCleaningPublisher(rabbitMqFactory, logger);
        await publisher.Publish(message);
    }

    private async Task<CleanerItemsCollection> FetchItemsToClean(
        ICleaner cleaner,
        NpgsqlConnection connection
    )
    {
        CleanerItemsSource cleanerItems = new(connection);
        CleanerItemsCollection items = await cleanerItems.FetchByTreshold(cleaner);
        return items;
    }

    private async Task<ICleaner> StartCleaner(ICleaner cleaner, NpgsqlConnection connection)
    {
        ICleaner started = cleaner.StartWork();
        await started
            .ProduceOutput()
            .PrintTo(new NpgSqlSavingCleanerVeil(connection))
            .BehaveAsync();
        return started;
    }

    private async Task InvalidateCache(ICleaner cleaner)
    {
        ICleanerCache cache = new CleanerStateCache(multiplexer);
        await cache.Invalidate(cleaner);
    }
}
