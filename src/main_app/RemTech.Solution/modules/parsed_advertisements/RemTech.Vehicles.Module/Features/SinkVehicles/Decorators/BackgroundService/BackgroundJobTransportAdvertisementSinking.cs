using Npgsql;
using RabbitMQ.Client;
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
        IIncreaseProcessedPublisher publisher,
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
                new RabbitVehicleSinkedSink(
                    publisher,
                    new ExceptionHandlingVehicleSinking(
                        new PgVehicleBrandSinking(
                            connection,
                            new PgVehicleKindSinking(
                                connection,
                                new PgLocationSinking(
                                    connection,
                                    new PgModelSinking(
                                        connection,
                                        new PgCharacteristicsSinking(
                                            connection,
                                            new PgVehicleSinking(
                                                connection,
                                                new EmptyVehicleSinking(),
                                                generator
                                            )
                                        ),
                                        generator
                                    ),
                                    generator
                                ),
                                generator
                            ),
                            generator
                        ),
                        logger
                    ),
                    logger
                )
            )
        );
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information(
            "Background job {0} is starting...",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _rabbitSink.OpenQueue(stoppingToken);
        _logger.Information(
            "Background job {0} has been started.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        stoppingToken.ThrowIfCancellationRequested();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information(
            "Background job {0} is stopping.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        await _rabbitSink.DisposeAsync();
        _logger.Information(
            "Background job {0} has been stopped.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
    }

    public override void Dispose()
    {
        _logger.Information(
            "Background job {0} is disposing.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
        Task rabbitStop = Task.Run(async () =>
        {
            await _rabbitSink.DisposeAsync();
        });
        rabbitStop.Wait();
        _logger.Information(
            "Background job {0} has been disposed.",
            nameof(BackgroundJobTransportAdvertisementSinking)
        );
    }
}
