using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;

namespace RemTech.RabbitMq.Abstractions;

public abstract class RabbitMqListenerHostService : BackgroundService
{
    private readonly RabbitMqConnectionSource _connectionSource;
    private RabbitMqListener? _listener;

    public RabbitMqListenerHostService(RabbitMqConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _listener ??= await GetListener()(_connectionSource, cancellationToken);
        AsyncEventHandler<BasicDeliverEventArgs> consumer = GetConsumerMethod(_listener);
        await _listener.Init(consumer, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Listener is consuming.");
        if (_listener != null) await _listener.Consume(stoppingToken);
        Console.WriteLine("Listener has consumed.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_listener != null) await _listener.DisposeAsync();
    }

    protected abstract AsyncEventHandler<BasicDeliverEventArgs> GetConsumerMethod(RabbitMqListener listener);
    protected abstract Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener();
}