using RabbitMQ.Client;

namespace Parsing.Vehicles.RabbitMq;

public sealed class RabbitMqChannel : IDisposable, IAsyncDisposable
{
    private readonly RabbitMqConnectionOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqChannel(RabbitMqConnectionOptions options)
    {
        _options = options;
    }

    public async Task<IChannel> Access()
    {
        ConnectionFactory connectionFactory = new ConnectionFactory()
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            Port = int.Parse(_options.Port),
        };
        _connection ??= await connectionFactory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();
        return _channel;
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.DisposeAsync();
        if (_connection != null)
            await _connection.DisposeAsync();
    }
}