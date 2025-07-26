using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Logging;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.RabbitMq;
using RemTech.Postgres.Adapter.Library;
using RemTech.RabbitMq.Adapter;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.BackgroundService;

public sealed class BackgroundJobTransportAdvertisementSinking : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly RabbitVehicleSink _rabbitSink;
    private readonly ICustomLogger _logger;

    public BackgroundJobTransportAdvertisementSinking(RabbitMqConnectionOptions options, PgConnectionSource connection, ICustomLogger logger)
    {
        _logger = logger;
        _rabbitSink = new RabbitVehicleSink(options, 
            new LoggingVehicleSink(logger, 
            new RabbitVehicleSinkedSink(
            new ExceptionHandlingVehicleSinking(
                new PgVehicleBrandSinking(connection,
                    new PgVehicleKindSinking(connection,
                        new PgLocationSinking(connection,
                            new PgModelSinking(connection,
                                new PgCharacteristicsSinking(connection,
                                    new PgVehicleSinking(connection,
                                        new EmptyVehicleSinking()))))))))
        ));
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Background job {0} is starting...", nameof(BackgroundJobTransportAdvertisementSinking));
        await _rabbitSink.OpenQueue("vehicles_sink", cancellationToken);
        _logger.Info("Background job {0} has been started.", nameof(BackgroundJobTransportAdvertisementSinking));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Background job {0} is stopping.", nameof(BackgroundJobTransportAdvertisementSinking));
        await _rabbitSink.DisposeAsync();
        _logger.Info("Background job {0} has been stopped.", nameof(BackgroundJobTransportAdvertisementSinking));
    }

    public override void Dispose()
    {
        _logger.Info("Background job {0} is disposing.", nameof(BackgroundJobTransportAdvertisementSinking));
        Task rabbitStop = Task.Run(async () => { await _rabbitSink.DisposeAsync();});
        rabbitStop.Wait();
        _logger.Info("Background job {0} has been disposed.", nameof(BackgroundJobTransportAdvertisementSinking));
    }
}