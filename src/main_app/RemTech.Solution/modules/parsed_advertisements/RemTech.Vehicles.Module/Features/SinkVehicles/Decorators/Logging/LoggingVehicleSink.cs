using Serilog;
using Status = RemTech.Core.Shared.Result.Status;

namespace RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.Logging;

internal sealed class LoggingVehicleSink(ILogger logger, ITransportAdvertisementSinking origin)
    : ITransportAdvertisementSinking
{
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        Status status = await origin.Sink(sink, ct);
        if (status.IsFailure)
        {
            logger.Error("Advertisement sink failed. Error: {0}.", status.Error.ErrorText);
        }
        return status;
    }
}
