using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators;

public sealed class EmptyVehicleSinking : ITransportAdvertisementSinking
{
    public Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        return Task.FromResult(Status.Success());
    }
}