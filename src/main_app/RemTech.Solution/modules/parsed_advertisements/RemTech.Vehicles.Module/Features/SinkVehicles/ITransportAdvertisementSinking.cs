using RemTech.Core.Shared.Result;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal interface ITransportAdvertisementSinking
{
    Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default);
}
