using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RemTech.Shared.Configuration.Options;

namespace Shared.Infrastructure.Module.Consumers;

public sealed class RabbitMqConnectionProvider
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options)
    {
        var optionValues = options.Value;
        _factory = new ConnectionFactory()
        {
            HostName = optionValues.HostName,
            UserName = optionValues.UserName,
            Password = optionValues.Password,
            Port = int.Parse(optionValues.Port),
        };
    }

    public async Task<IConnection> ProvideConnection(CancellationToken ct = default)
    {
        _connection ??= await _factory.CreateConnectionAsync(ct);
        return _connection;
    }

    public async Task<IChannel> ProvideChannel(CancellationToken ct = default)
    {
        _connection ??= await ProvideConnection(ct);
        return await _connection.CreateChannelAsync(cancellationToken: ct);
    }
}