namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal interface ITransportAdvertisementSinking
{
    Task<Result.Pattern.Result> Sink(IVehicleJsonSink sink, CancellationToken ct = default);
}
