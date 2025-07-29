using RemTech.Result.Library;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

public sealed class EmptyVehicleSinking : ITransportAdvertisementSinking
{
    public Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        return Task.FromResult(Status.Success());
    }
}
