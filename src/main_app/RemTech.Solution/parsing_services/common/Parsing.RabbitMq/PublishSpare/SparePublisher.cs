using System.Text;
using System.Text.Json;
using Parsing.RabbitMq.Common;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.PublishSpare;

public sealed class SparePublisher(IConnection connection) : ISparePublisher
{
    public async Task Publish(SpareSinkMessage message)
    {
        await using IChannel prepared = await new ChannelSinkPreparation(
            await connection.CreateChannelAsync()
        ).Prepared(
            RabbitMqScraperConstants.AdvertisementsExchange,
            RabbitMqScraperConstants.SparesCreateQueue
        );
        await prepared.BasicPublishAsync(
            RabbitMqScraperConstants.AdvertisementsExchange,
            RabbitMqScraperConstants.SparesCreateQueue,
            body: PrepareBody(message)
        );
    }

    private static ReadOnlyMemory<byte> PrepareBody(SpareSinkMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }
}
