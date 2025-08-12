using RemTech.Core.Shared.Result;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

internal sealed class EmptyVehicleSinking : ITransportAdvertisementSinking
{
    public Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        return Task.FromResult(Status.Success());
    }
}
