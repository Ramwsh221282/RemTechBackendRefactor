using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.FinishParserLink;

internal sealed class ParserLinkFinishedMessagePublisher(IConnection connection)
    : IParserLinkFinishedMessagePublisher
{
    public async Task Publish(FinishedParserLinkMessage message)
    {
        await using IChannel channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(
            RabbitMqScraperConstants.ScrapersExchange,
            ExchangeType.Direct,
            false,
            false,
            null
        );
        await channel.QueueDeclareAsync(
            RabbitMqScraperConstants.ScrapersFinishLinkQueue,
            false,
            false,
            false,
            null
        );
        await channel.QueueBindAsync(
            RabbitMqScraperConstants.ScrapersFinishLinkQueue,
            RabbitMqScraperConstants.ScrapersExchange,
            RabbitMqScraperConstants.ScrapersFinishLinkQueue,
            null
        );
        string json = JsonSerializer.Serialize(message);
        ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(json);
        await channel.BasicPublishAsync(
            RabbitMqScraperConstants.ScrapersExchange,
            RabbitMqScraperConstants.ScrapersFinishLinkQueue,
            body: bytes
        );
    }
}
