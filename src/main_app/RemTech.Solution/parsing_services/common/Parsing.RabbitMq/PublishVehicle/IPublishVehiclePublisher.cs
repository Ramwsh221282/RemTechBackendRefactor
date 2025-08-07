namespace Parsing.RabbitMq.PublishVehicle;

public interface IPublishVehiclePublisher
{
    Task Publish(VehiclePublishMessage message);
}
