namespace Parsing.RabbitMq.PublishVehicle;

public sealed record VehiclePublishMessage(
    ParserBody Parser,
    ParserLinkBody Link,
    VehicleBody Vehicle
);
