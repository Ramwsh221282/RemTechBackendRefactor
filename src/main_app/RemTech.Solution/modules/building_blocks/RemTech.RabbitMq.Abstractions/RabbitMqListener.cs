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
    
    public async Task Consume(CancellationToken ct)
    {
        await DeclareQueue(ct);
        await DeclareExchange(ct);
        await BindQueue(ct);
        await BeginConsume(ct);
    }

    public RabbitMqListener(AsyncEventHandler<BasicDeliverEventArgs> handler, DeclareQueueArgs args, IChannel channel)
    {
        _channel = channel;
        _args = args;
        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += handler;
    }

    private async Task DeclareQueue(CancellationToken ct)
    {
        await _channel.QueueDeclareAsync(
            _args.QueueName, 
            durable: true, 
            exclusive: false,
            autoDelete: false,
            passive: false,
            arguments: null,
            noWait: false,
            cancellationToken: ct);   
    }

    private async Task DeclareExchange(CancellationToken ct)
    {
        await _channel.ExchangeDeclareAsync(
            _args.Exchange, 
            _args.ExchangeType, 
            durable: true, 
            autoDelete: false,
            arguments: null,
            passive: false,
            noWait: false,
            cancellationToken: ct);
    }

    private async Task BindQueue(CancellationToken ct)
    {
        await _channel.QueueBindAsync(
            _args.QueueName, 
            _args.Exchange,
            _args.RoutingKey, 
            cancellationToken: ct);
    }

    private async Task BeginConsume(CancellationToken ct)
    {
        await _channel.BasicConsumeAsync(
            _args.QueueName, 
            autoAck: true, 
            consumer: _consumer, 
            cancellationToken: ct);
    }
}