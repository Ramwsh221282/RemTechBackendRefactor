using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement;

public interface ITransportAdvertisementSinking
{
    Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default);
}