using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RemTech.RabbitMq.Adapter;

public sealed class RabbitAcceptPoint : IDisposable, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly AsyncEventingBasicConsumer _consumer;
    private readonly string _queueName;
    private readonly AsyncEventHandler<BasicDeliverEventArgs> _handler;
    
    public RabbitAcceptPoint(
        IChannel channel, 
        string queueName, 
        AsyncEventHandler<BasicDeliverEventArgs> handler)
    {
        _channel = channel;
        _queueName = queueName;
        _consumer = new AsyncEventingBasicConsumer(channel);
        _handler = handler;
    }

    public async Task Acknowledge(BasicDeliverEventArgs ea, CancellationToken ct = default)
    {
        await _channel.BasicAckAsync(ea.DeliveryTag, false, ct);
    }
    
    public async Task StartConsuming(CancellationToken ct = default)
    {
        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false, cancellationToken: ct);
        _consumer.ReceivedAsync += _handler;
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: _consumer, cancellationToken: ct);
    }

    public void Dispose()
    {
        _consumer.ReceivedAsync -= _handler;
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _consumer.ReceivedAsync -= _handler;
        await _channel.QueueDeleteAsync(_queueName);
        await _channel.DisposeAsync();
    }
}