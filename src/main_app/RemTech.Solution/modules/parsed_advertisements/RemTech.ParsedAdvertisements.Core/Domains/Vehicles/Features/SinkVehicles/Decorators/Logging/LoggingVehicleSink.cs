using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkVehicles.Decorators.Logging;

public sealed class LoggingVehicleSink : ITransportAdvertisementSinking
{
    private readonly ILogger _logger;
    private readonly ITransportAdvertisementSinking _origin;

    public LoggingVehicleSink(ILogger logger, ITransportAdvertisementSinking origin)
    {
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        _logger.Information("Sinking advertisement...");
        Status status = await _origin.Sink(sink, ct);
        if (status.IsSuccess)
        {
            _logger.Information("Advertisement sink success.");
        }
        else
        {
            _logger.Error("Advertisement sink failed. Error: {0}.", status.Error.ErrorText);
        }

        return status;
    }
}