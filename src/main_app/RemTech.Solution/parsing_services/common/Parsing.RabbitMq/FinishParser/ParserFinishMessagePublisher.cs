using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.FinishParser;

internal sealed class ParserFinishMessagePublisher(IConnection connection)
    : IParserFinishMessagePublisher
{
    public async Task Publish(ParserFinishedMessage message)
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
            RabbitMqScraperConstants.ScrapersFinishQueue,
            false,
            false,
            false,
            null
        );
        await channel.QueueBindAsync(
            RabbitMqScraperConstants.ScrapersFinishQueue,
            RabbitMqScraperConstants.ScrapersExchange,
            RabbitMqScraperConstants.ScrapersFinishQueue,
            null
        );
        string json = JsonSerializer.Serialize(message);
        ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(json);
        await channel.BasicPublishAsync(
            RabbitMqScraperConstants.ScrapersExchange,
            RabbitMqScraperConstants.ScrapersFinishQueue,
            body: bytes
        );
    }
}
