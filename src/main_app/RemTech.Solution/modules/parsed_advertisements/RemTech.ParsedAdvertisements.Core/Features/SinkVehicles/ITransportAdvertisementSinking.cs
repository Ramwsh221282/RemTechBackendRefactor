using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Features.SinkVehicles;

public interface ITransportAdvertisementSinking
{
    Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default);
}
