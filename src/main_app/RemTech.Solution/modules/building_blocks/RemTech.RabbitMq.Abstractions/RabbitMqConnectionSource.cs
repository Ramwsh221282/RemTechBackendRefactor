using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RemTech.RabbitMq.Abstractions;

public sealed record DeclareQueueArgs(
    string QueueName, 
    string Exchange, 
    string RoutingKey, 
    string ExchangeType);

public sealed class RabbitMqConnectionSource
{
    private readonly RabbitMqConnectionOptions _options;
    private Lazy<Task<IConnection>> _lazyConnection;
    
    public RabbitMqConnectionSource(IOptions<RabbitMqConnectionOptions> options)
    {
        _options = options.Value;
        _lazyConnection = new Lazy<Task<IConnection>>(async () =>
        {
            RabbitMqConnectionOptions rabbitOptions = options.Value;
            ConnectionFactory factory = new()
            {
                HostName = rabbitOptions.Hostname,
                Password = rabbitOptions.Password,
                Port = rabbitOptions.Port,
                UserName = rabbitOptions.Username,
            };
            return await factory.CreateConnectionAsync();
        });
    }

    public async Task<IConnection> GetConnection(CancellationToken ct)
    {
        IConnection connection = await _lazyConnection.Value;
        if (connection.IsOpen) return connection;
        _lazyConnection = new Lazy<Task<IConnection>>(async () =>
        {
            ConnectionFactory factory = new()
            {
                HostName = _options.Hostname,
                Password = _options.Password,
                Port = _options.Port,
                UserName = _options.Username,
            };
            return await factory.CreateConnectionAsync(ct);
        });
        return await _lazyConnection.Value;
    }

    public async Task<RabbitMqPublisher> CreatePublisher(string exchange, string routingKey, CancellationToken ct)
    {
        IConnection connection = await GetConnection(ct);
        return new RabbitMqPublisher(await connection.CreateChannelAsync(cancellationToken: ct), exchange, routingKey);
    }

    public async Task<RabbitMqListener> CreateListener(DeclareQueueArgs args, CancellationToken ct)
    {
        IConnection connection = await GetConnection(ct);
        IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        return new RabbitMqListener(args, channel);
    }
}