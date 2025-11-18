using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RemTech.RabbitMq.Abstractions;

public sealed class RabbitMqListener : IDisposable, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly DeclareQueueArgs _args;
    private readonly AsyncEventingBasicConsumer _consumer;
    
    public void Dispose() => _channel.Dispose();

    public async ValueTask DisposeAsync() => await _channel.DisposeAsync();
    
    public async Task Init(AsyncEventHandler<BasicDeliverEventArgs> consumer, CancellationToken ct)
    {
        await _channel.QueueDeclareAsync(_args.QueueName, false, false, false, cancellationToken: ct);
        await _channel.ExchangeDeclareAsync(_args.Exchange, _args.ExchangeType, false, false, cancellationToken: ct);
        await _channel.QueueBindAsync(_args.QueueName, _args.Exchange, _args.RoutingKey, cancellationToken: ct);
        _consumer.ReceivedAsync += consumer;
    }

    public async Task Acknowledge(BasicDeliverEventArgs args)
    {
        await _channel.BasicAckAsync(args.DeliveryTag, false);
    }
    
    public async Task Consume(CancellationToken ct)
    {
        await _channel.BasicConsumeAsync(_args.QueueName, false, consumer: _consumer, cancellationToken: ct);
    }

    public RabbitMqListener(DeclareQueueArgs args, IChannel channel)
    {
        _channel = channel;
        _args = args;
        _consumer = new AsyncEventingBasicConsumer(_channel);
    }
}