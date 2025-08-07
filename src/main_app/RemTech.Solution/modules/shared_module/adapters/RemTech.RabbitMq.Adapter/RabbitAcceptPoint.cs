using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RemTech.RabbitMq.Adapter;

public sealed class RabbitAcceptPoint(
    IChannel channel,
    AsyncEventHandler<BasicDeliverEventArgs> handler
) : IDisposable, IAsyncDisposable
{
    private readonly AsyncEventingBasicConsumer _consumer = new(channel);

    public async Task Acknowledge(BasicDeliverEventArgs ea, CancellationToken ct = default)
    {
        await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
    }

    public async Task StartConsuming(CancellationToken ct = default)
    {
        await channel.ExchangeDeclareAsync(
            "advertisements",
            ExchangeType.Direct,
            false,
            false,
            cancellationToken: ct
        );
        await channel.QueueDeclareAsync(
            queue: "vehicles",
            durable: false,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct
        );
        await channel.QueueBindAsync(
            "vehicles",
            "advertisements",
            "vehicles",
            cancellationToken: ct
        );
        _consumer.ReceivedAsync += handler;
        await channel.BasicConsumeAsync(
            queue: "vehicles",
            autoAck: false,
            consumer: _consumer,
            cancellationToken: ct
        );
    }

    public void Dispose()
    {
        _consumer.ReceivedAsync -= handler;
        channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _consumer.ReceivedAsync -= handler;
        await channel.DisposeAsync();
    }
}
