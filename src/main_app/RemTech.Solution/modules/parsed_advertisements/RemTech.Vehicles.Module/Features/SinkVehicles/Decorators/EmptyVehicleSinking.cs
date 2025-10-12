namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators;

internal sealed class EmptyVehicleSinking : ITransportAdvertisementSinking
{
    public Task<Result.Pattern.Result> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Pattern.Result.Success());
    }
}
