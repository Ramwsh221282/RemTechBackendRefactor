using Parsing.Vehicles.Common.Json;
using Parsing.Vehicles.Common.ParsedVehicles;
using RemTech.RabbitMq.Adapter;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing;

public sealed class RabbitPublishingParsedVehicle
{
    private readonly IParsedVehicle _vehicle;
    private readonly ILogger _logger;
    private readonly RabbitSendPoint _point;

    public RabbitPublishingParsedVehicle(ILogger logger, IParsedVehicle vehicle, RabbitSendPoint point)
    {
        _vehicle = vehicle;
        _point = point;
        _logger = logger;
    }

    public async Task SendAsync()
    {
        if (await new ValidatingParsedVehicle(_vehicle).IsValid())
        {
            await _point.SendJson(new ParsedVehicleInfo(
                await _vehicle.Identity(),
                await _vehicle.Kind(),
                await _vehicle.Brand(),
                await _vehicle.Model(),
                await _vehicle.Price(),
                await _vehicle.Characteristics(),
                await _vehicle.Photos(),
                await _vehicle.Geo(),
                new ParsedVehicleParser("Test Parser", "Test Type", "Test Link")
            ).Json().Read());
            _logger.Information("Vehicle sended to rabbit mq.");
            return;
        }
        _logger.Warning("Vehicle was not sent to rabbit mq. Not valid.");
    }
}