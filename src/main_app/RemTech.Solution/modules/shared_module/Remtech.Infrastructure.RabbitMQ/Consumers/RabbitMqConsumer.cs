using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Remtech.Infrastructure.RabbitMQ.Consumers;

public sealed class RabbitMqConsumer : IAsyncDisposable, IDisposable
{
    private readonly IConnection _connection;
    private string _queue = string.Empty;
    private bool _autoAck;
    private AsyncEventingBasicConsumer _consumer = null!;
    public IChannel Channel { get; }

    public RabbitMqConsumer(IChannel channel, IConnection connection)
    {
        Channel = channel;
        _connection = connection;
    }

    public RabbitMqConsumer AttachListener(IRabbitMqListener listener)
    {
        _consumer = new AsyncEventingBasicConsumer(Channel);
        _consumer.ReceivedAsync += listener.HandleMessage;
        return this;
    }

    public async Task Consume(CancellationToken ct = default)
    {
        await Channel.BasicConsumeAsync(
            _queue,
            autoAck: _autoAck,
            consumer: _consumer,
            cancellationToken: ct
        );
    }

    public void AddQueue(string queue) => _queue = queue;

    public void AutoAck() => _autoAck = true;

    public async ValueTask DisposeAsync()
    {
        await Channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    public void Dispose()
    {
        Channel.Dispose();
        _connection.Dispose();
    }
}
