using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RemTech.RabbitMq.Adapter;

public sealed class RabbitMqChannel : IDisposable, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqChannel(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public IChannel Access()
    {
        return _channel;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }

    public async Task Acknowledge(BasicDeliverEventArgs ea, CancellationToken cancel)
    {
        await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancel);
    }

    public async Task<RabbitAcceptPoint> MakeAcceptPoint(
        AsyncEventHandler<BasicDeliverEventArgs> handler
    )
    {
        return new RabbitAcceptPoint(_channel, handler);
    }

    public async Task<RabbitSendPoint> MakeSendPoint(string queueName)
    {
        return new RabbitSendPoint(_channel, queueName);
    }
}
