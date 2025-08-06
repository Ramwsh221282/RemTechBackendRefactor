using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq;

internal sealed class CreateNewParserPublisher(ConnectionFactory connectionFactory)
    : ICreateNewParserPublisher
{
    public async Task<bool> SendCreateNewParser(
        CreateNewParserMessage message,
        CancellationToken ct = default
    )
    {
        await using IConnection connection = await connectionFactory.CreateConnectionAsync(ct);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        string serialized = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(serialized);
        try
        {
            await channel.BasicPublishAsync(
                string.Empty,
                "new_parsers",
                body: body,
                cancellationToken: ct
            );
            return true;
        }
        catch
        {
            return false;
        }
    }
}
