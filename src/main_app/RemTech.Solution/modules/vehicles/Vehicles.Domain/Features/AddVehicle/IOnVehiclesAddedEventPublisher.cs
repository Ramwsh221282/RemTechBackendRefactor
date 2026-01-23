namespace Vehicles.Domain.Features.AddVehicle;

public interface IOnVehiclesAddedEventPublisher
{
    public Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}
