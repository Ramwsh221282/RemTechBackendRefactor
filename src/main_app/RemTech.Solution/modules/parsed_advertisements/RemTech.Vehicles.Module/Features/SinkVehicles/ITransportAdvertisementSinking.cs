using RemTech.Result.Library;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

public interface ITransportAdvertisementSinking
{
    Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default);
}
