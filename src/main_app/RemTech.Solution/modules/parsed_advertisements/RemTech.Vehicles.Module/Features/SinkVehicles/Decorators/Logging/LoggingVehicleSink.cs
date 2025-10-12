using Serilog;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Logging;

internal sealed class LoggingVehicleSink(ILogger logger, ITransportAdvertisementSinking origin)
    : ITransportAdvertisementSinking
{
    public async Task<Result.Pattern.Result> Sink(
        IVehicleJsonSink sink,
        CancellationToken ct = default
    )
    {
        Result.Pattern.Result result = await origin.Sink(sink, ct);
        if (result.IsFailure)
        {
            logger.Error("Advertisement sink failed. Error: {0}.", result.Error.ErrorText);
        }
        return result;
    }
}
