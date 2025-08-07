using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.CreateParser;

internal sealed class CreateNewParserPublisher(IConnection connection) : ICreateNewParserPublisher
{
    public async Task<bool> SendCreateNewParser(
        CreateNewParserMessage message,
        CancellationToken ct = default
    )
    {
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: ct);
        string serialized = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(serialized);
        try
        {
            await channel.BasicPublishAsync(
                RabbitMqScraperConstants.ScrapersExchange,
                RabbitMqScraperConstants.ScrapersCreateQueue,
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
