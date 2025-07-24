using Parsing.Vehicles.Common.Json;
using Parsing.Vehicles.Common.ParsedVehicles;
using RabbitMQ.Client;

namespace Parsing.Vehicles.RabbitMq;

public sealed class RabbitParsedVehicle : IPublishingParsedVehicle
{
    private readonly RabbitMqChannel _channel;
    private readonly ParsedVehiclePublishingDestination _destination;
    

    public RabbitParsedVehicle(RabbitMqChannel channel, ParsedVehiclePublishingDestination destination)
    {
        _channel = channel;
        _destination = destination;
    }

    public async Task<IParsedVehicle> Publish(ParsedVehicleParser parser, IParsedVehicle vehicle)
    {
        ParsedVehicleInfo info = await new ParsedVehicleToPublish(vehicle, parser).InfoToPublish();
        IChannel rabbitChannel = await _channel.Access();
        await rabbitChannel.QueueDeclareAsync(
                _destination.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        await rabbitChannel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _destination.QueueName, 
            body: info.Json().Bytes().Read());
        return vehicle;
    }
}