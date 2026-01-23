using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Vehicles.Domain.Features.AddVehicle;

public sealed class AddVehicleEventTransporter(IOnVehiclesAddedEventPublisher publisher)
    : IEventTransporter<AddVehicleCommand, (Guid, int)>
{
    public async Task Transport((Guid, int) result, CancellationToken ct = default)
    {
        if (result.Item1 == Guid.Empty)
            return;
        if (result.Item2 == 0)
            return;
        await publisher.Publish(result.Item1, result.Item2, ct);
    }
}
