using RemTech.Configuration.Library;

namespace Parsing.Vehicles.RabbitMq;

public sealed class ParsedVehiclePublishingDestination
{
    public string QueueName { get; }
    public string RoutingKey { get; }

    public ParsedVehiclePublishingDestination(string filePath)
    {
        IConfig config = new JsonConfig(filePath);
        IConfigCursor cursor = config.Cursor(nameof(ParsedVehiclePublishingDestination));
        QueueName = cursor.GetOption(nameof(QueueName));
        RoutingKey = cursor.GetOption(nameof(RoutingKey));
    }

    public ParsedVehiclePublishingDestination(string queueName, string routingKey)
    {
        QueueName = queueName;
        RoutingKey = routingKey;
    }
}