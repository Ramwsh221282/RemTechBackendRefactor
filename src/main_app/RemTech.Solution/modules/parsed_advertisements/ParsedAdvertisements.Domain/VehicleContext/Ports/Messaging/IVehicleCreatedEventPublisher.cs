using ParsedAdvertisements.Domain.VehicleContext.Events;

namespace ParsedAdvertisements.Domain.VehicleContext.Ports.Messaging;

public interface IVehicleCreatedEventPublisher
{
    Task Publish(VehicleCreatedEvent @event);
}