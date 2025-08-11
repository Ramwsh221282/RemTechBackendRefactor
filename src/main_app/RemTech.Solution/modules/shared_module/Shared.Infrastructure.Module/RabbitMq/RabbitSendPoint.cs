using System.Text;
using RabbitMQ.Client;

namespace Shared.Infrastructure.Module.RabbitMq;

public sealed class RabbitSendPoint : IDisposable, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly string _queueName;
    private bool _queueDeclared;

    public RabbitSendPoint(IChannel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    public async Task SendJson(string json, CancellationToken ct = default)
    {
        await DeclareQueue(ct);
        byte[] body = Encoding.UTF8.GetBytes(json);
        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _queueName,
            body: body,
            cancellationToken: ct
        );
    }

    private async Task DeclareQueue(CancellationToken ct)
    {
        if (!_queueDeclared)
        {
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                cancellationToken: ct
            );
            _queueDeclared = true;
        }
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
    }
}
