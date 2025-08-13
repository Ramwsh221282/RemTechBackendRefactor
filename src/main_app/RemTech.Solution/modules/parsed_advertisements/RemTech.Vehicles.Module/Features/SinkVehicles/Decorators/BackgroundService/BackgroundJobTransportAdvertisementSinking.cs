using Brands.Module.Public;
using Categories.Module.Public;
using GeoLocations.Module.Features.Querying;
using Models.Module.Public;
using Npgsql;
using RabbitMQ.Client;
using RemTech.ContainedItems.Module.Features.MessageBus;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Logging;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Postgres;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.RabbitMq;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;
using Serilog;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.BackgroundService;

public sealed class BackgroundJobTransportAdvertisementSinking
    : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly RabbitVehicleSink _rabbitSink;
    private readonly ILogger _logger;

    public BackgroundJobTransportAdvertisementSinking(
        IGeoLocationQueryService locationsQuery,
        IIncreaseProcessedPublisher processedPublisher,
        IAddContainedItemsPublisher containedPublisher,
        IBrandsPublicApi brandsApi,
        IModelPublicApi modelApi,
        ICategoryPublicApi categoryApi,
        ConnectionFactory rabbitConnectionFactory,
        NpgsqlDataSource connection,
        IEmbeddingGenerator generator,
        ILogger logger
    )
    {
        _logger = logger;
        _rabbitSink = new RabbitVehicleSink(
            rabbitConnectionFactory,
            new LoggingVehicleSink(
                logger,
                new RabbitVehicleSinkLogic(
                    processedPublisher,
                    containedPublisher,
                    logger,
                    new ExceptionHandlingVehicleSinking(
                        logger,
                        new BrandSpecifying(
                            brandsApi,
                            new CategorySpecifying(
                                categoryApi,
                                new PgLocationSinking(
                                    locationsQuery,
                                    new PgModelSinking(
                                        modelApi,
                                        new PgCharacteristicsSinking(
                                            new PgVehicleSinking(
                                                connection,
                                                generator,
                                                new EmptyVehicleSinking()
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        );
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information(
            "{0} is starting...",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbitSink.OpenQueue(stoppingToken);
        _logger.Information(
            "{0} has been started.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        stoppingToken.ThrowIfCancellationRequested();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("{0} is stopping.", nameof(BackgroundJobTransportAdvertisementSinking));
        await _rabbitSink.DisposeAsync();
        _logger.Information(
            "{0} has been stopped.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
    }

    public override void Dispose()
    {
        _logger.Information(
            "{0} is disposing.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        Task rabbitStop = Task.Run(async () =>
        {
            await _rabbitSink.DisposeAsync();
        });
        rabbitStop.Wait();
        _logger.Information(
            "{0} has been disposed.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
    }
}
