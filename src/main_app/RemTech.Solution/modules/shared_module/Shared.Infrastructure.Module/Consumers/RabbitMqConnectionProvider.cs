using RabbitMQ.Client;
using RemTech.Shared.Configuration;

namespace Shared.Infrastructure.Module.Consumers;

public sealed class RabbitMqConnectionProvider
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnectionProvider(RabbitMqOptions options)
    {
        _factory = new ConnectionFactory()
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            Port = int.Parse(options.Port),
        };
    }

    public async Task<IConnection> ProvideConnection(CancellationToken ct = default)
    {
        _connection ??= await _factory.CreateConnectionAsync(ct);
        return _connection;
    }
}
