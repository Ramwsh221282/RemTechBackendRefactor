using System.Text;
using RabbitMQ.Client;

namespace RemTech.RabbitMq.Abstractions;

public sealed class RabbitMqPublisher : IDisposable, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly string _exchange;
    private readonly string _routingKey;
    private readonly string _correlationId = Guid.NewGuid().ToString();
    private readonly bool _isMandatory = true;
    private readonly TaskCompletionSource<bool> _tcs = new();

    public async Task Publish(string body, CancellationToken ct)
    {
        await _channel.BasicPublishAsync(
            _exchange, 
            _routingKey,
            _isMandatory,
            basicProperties: new BasicProperties { CorrelationId = _correlationId, },
            CreateMessageBody(body),
            cancellationToken: ct);
    }

    public async Task<bool> EnsureMessageDelivered(int waitSeconds)
    {
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(waitSeconds));
        try
        {
            return await _tcs.Task.WaitAsync(cts.Token);
        }
        catch(OperationCanceledException)
        {
            return false;
        }
    }
    
    private ReadOnlyMemory<byte> CreateMessageBody(string body)
    {
        ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(body);
        return bytes;
    }
    
    public void Dispose()
    {
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
    }

    public RabbitMqPublisher(IChannel channel, string exchange, string routingKey)
    {
        _channel = channel;
        _exchange = exchange;
        _routingKey = routingKey;
        _channel.BasicReturnAsync += (_, @event) =>
        {
            string? correlationId = @event.BasicProperties.CorrelationId;
            if (!string.IsNullOrWhiteSpace(correlationId) && _correlationId == correlationId)
            {
                _tcs.SetResult(false);
            }
            return Task.CompletedTask;
        };
        _channel.BasicAcksAsync += (_, @event) =>
        {
            _tcs.SetResult(true);
            return Task.CompletedTask;
        };
    }
}