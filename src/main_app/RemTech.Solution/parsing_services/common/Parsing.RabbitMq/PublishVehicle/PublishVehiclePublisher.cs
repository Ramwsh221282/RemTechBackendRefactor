using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Parsing.RabbitMq.PublishVehicle;

internal sealed class PublishVehiclePublisher(IConnection connection) : IPublishVehiclePublisher
{
    public async Task Publish(VehiclePublishMessage message)
    {
        await using IChannel channel = await connection.CreateChannelAsync();
        await PreparePublishing(channel);
        ReadOnlyMemory<byte> body = PrepareBody(message);
        await channel.BasicPublishAsync(
            RabbitMqScraperConstants.AdvertisementsExchange,
            RabbitMqScraperConstants.VehiclesCreateQueue,
            false,
            body: body
        );
    }

    private static ReadOnlyMemory<byte> PrepareBody(VehiclePublishMessage message)
    {
        string json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }

    private static async Task PreparePublishing(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(
            RabbitMqScraperConstants.AdvertisementsExchange,
            ExchangeType.Direct,
            false,
            false,
            null
        );
        await channel.QueueDeclareAsync(
            RabbitMqScraperConstants.VehiclesCreateQueue,
            false,
            false,
            false,
            null
        );
        await channel.QueueBindAsync(
            RabbitMqScraperConstants.VehiclesCreateQueue,
            RabbitMqScraperConstants.AdvertisementsExchange,
            RabbitMqScraperConstants.VehiclesCreateQueue,
            null
        );
    }
}
