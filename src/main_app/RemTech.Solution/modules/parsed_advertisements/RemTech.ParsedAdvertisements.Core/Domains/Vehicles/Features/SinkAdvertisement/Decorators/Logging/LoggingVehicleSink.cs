using RemTech.Logging.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement.Decorators.Logging;

public sealed class LoggingVehicleSink : ITransportAdvertisementSinking
{
    private readonly ICustomLogger _logger;
    private readonly ITransportAdvertisementSinking _origin;

    public LoggingVehicleSink(ICustomLogger logger, ITransportAdvertisementSinking origin)
    {
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<Status> Sink(IVehicleJsonSink sink, CancellationToken ct = default)
    {
        _logger.Info("Sinking advertisement...");
        Status status = await _origin.Sink(sink, ct);
        if (status.IsSuccess)
        {
            _logger.Info("Advertisement sink success.");
        }
        else
        {
            _logger.Error("Advertisement sink failed. Error: {0}.", status.Error.ErrorText);
        }

        return status;
    }
}