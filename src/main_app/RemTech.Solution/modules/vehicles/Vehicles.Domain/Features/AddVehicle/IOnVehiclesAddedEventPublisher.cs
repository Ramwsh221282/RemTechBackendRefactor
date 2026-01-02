namespace Vehicles.Domain.Features.AddVehicle;

public interface IOnVehiclesAddedEventPublisher
{
    Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}