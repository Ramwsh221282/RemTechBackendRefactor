using Microsoft.Extensions.Hosting;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public abstract class RabbitMqListenerHostService : BackgroundService
{
    private readonly RabbitMqConnectionSource _connectionSource;

    public RabbitMqListenerHostService(RabbitMqConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RabbitMqListener listener = await GetListener()(_connectionSource, stoppingToken);
        await listener.Consume(stoppingToken);
    }
    
    protected abstract Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener();
}