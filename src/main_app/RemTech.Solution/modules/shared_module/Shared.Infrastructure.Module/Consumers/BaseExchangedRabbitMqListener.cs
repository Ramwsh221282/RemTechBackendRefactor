using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.Infrastructure.Module.Consumers;

public abstract class BaseExchangedRabbitMqListener : BackgroundService, IRabbitMqListener
{
    protected readonly RabbitMqConsumerConfigurer Configurer;
    private IChannel _channel = null!;

    protected BaseExchangedRabbitMqListener(RabbitMqConnectionProvider provider)
    {
        Configurer = new RabbitMqConsumerConfigurer(provider);
    }

    public abstract void Configure();

    public abstract Task HandleMessage(object sender, BasicDeliverEventArgs eventArgs);

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Configure();
        await base.StartAsync(cancellationToken);
    }

    protected async Task Acknowledge(
        BasicDeliverEventArgs eventArgs,
        CancellationToken ct = default
    )
    {
        await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, ct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using RabbitMqConsumer consumer = await Configurer.ConfigureExchangedConsumer(
            ct: stoppingToken
        );
        _channel = consumer.Channel;

        consumer.AttachListener(this);
        await consumer.Consume(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
