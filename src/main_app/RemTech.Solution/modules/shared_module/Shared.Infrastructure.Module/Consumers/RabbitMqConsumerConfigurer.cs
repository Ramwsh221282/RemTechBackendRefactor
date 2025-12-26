using RabbitMQ.Client;

namespace Shared.Infrastructure.Module.Consumers;

public sealed class RabbitMqConsumerConfigurer
{
    private readonly RabbitMqConnectionProvider _provider;
    private bool _autoAck;

    public RabbitMqConsumerConfigurer(RabbitMqConnectionProvider provider)
    {
        _provider = provider;
        Exchange = new RabbitMqExchangeSettings();
        Queue = new RabbitMqQueueSettings();
    }

    public RabbitMqExchangeSettings Exchange { get; }
    public RabbitMqQueueSettings Queue { get; }

    public RabbitMqConsumerConfigurer WithAutoAck()
    {
        _autoAck = true;
        return this;
    }

    public async Task<RabbitMqConsumer> ConfigureConsumer(CancellationToken ct = default)
    {
        IConnection connection = await _provider.ProvideConnection(ct);
        IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await Queue.Declare(channel, ct);
        RabbitMqConsumer consumer = new RabbitMqConsumer(channel, connection);
        Queue.AddQueueName(consumer);
        if (_autoAck)
            consumer.AutoAck();
        return consumer;
    }

    public async Task<RabbitMqConsumer> ConfigureExchangedConsumer(CancellationToken ct = default)
    {
        IConnection connection = await _provider.ProvideConnection(ct);
        IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        await Queue.Declare(channel, ct);
        await Exchange.Declare(channel, ct);
        await Exchange.BindQueue(channel, Queue, ct);
        RabbitMqConsumer consumer = new RabbitMqConsumer(channel, connection);
        Queue.AddQueueName(consumer);
        if (_autoAck)
            consumer.AutoAck();
        return consumer;
    }
}
