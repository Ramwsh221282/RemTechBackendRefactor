using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Scrapers.Module.Features.EnableParser.Models;

namespace Scrapers.Module.Features.EnableParser.RabbitMq;

internal sealed class RabbitMqParsersStorage(
    ConnectionFactory factory,
    IEnabledParsersStorage origin
) : IEnabledParsersStorage
{
    public async Task<EnabledParser> Save(
        EnabledParser parser,
        CancellationToken cancellationToken = default
    )
    {
        EnabledParser enabled = await origin.Save(parser, cancellationToken);
        await using IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
        await using IChannel channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: $"{enabled.Name}_{enabled.Type}",
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(enabled)),
            cancellationToken: cancellationToken
        );
        return enabled;
    }
}
