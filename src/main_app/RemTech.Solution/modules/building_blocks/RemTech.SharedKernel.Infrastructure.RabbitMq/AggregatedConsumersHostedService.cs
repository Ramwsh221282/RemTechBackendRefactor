using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class AggregatedConsumersHostedService(
    Serilog.ILogger logger,
    IEnumerable<IConsumer> consumers,
    RabbitMqConnectionSource connectionSource) : BackgroundService
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<AggregatedConsumersHostedService>();
    private IEnumerable<IConsumer> Consumers { get; } = consumers;
    private RabbitMqConnectionSource ConnectionSource { get; } = connectionSource;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.Information("Initializing aggregated consumers.");
        IConnection connection = await ConnectionSource.GetConnection(stoppingToken);
        IEnumerable<Task> initializeTasks = Consumers.Select(c => Task.Run(() => c.InitializeChannel(connection, stoppingToken)));
        await Task.WhenAll(initializeTasks);
        Logger.Information("Aggregated consumers initialized. Starting consumers.");
        IEnumerable<Task> startTasks = Consumers.Select(c => Task.Run(() => c.StartConsuming(stoppingToken)));
        await Task.WhenAll(startTasks);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.Information("Shutting down parsers control event listeners.");
        IEnumerable<Task> tasks = Consumers.Select(c => Task.Run(() => c.Shutdown(cancellationToken)));
        await Task.WhenAll(tasks);
        Logger.Information("Parsers control event listeners shut down.");
    }
}