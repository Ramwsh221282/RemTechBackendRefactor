using Parsing.Vehicles.Common.Json;
using Parsing.Vehicles.Common.ParsedVehicles;

namespace Parsing.Vehicles.RabbitMq;

public sealed class ParsedVehicleToPublish
{
    private readonly IParsedVehicle _vehicle;
    private readonly ParsedVehicleParser _parser;

    public ParsedVehicleToPublish(IParsedVehicle vehicle, ParsedVehicleParser parser)
    {
        _vehicle = vehicle;
        _parser = parser;
    }

    public async Task<ParsedVehicleInfo> InfoToPublish()
    {
        return new ParsedVehicleInfo(
            await _vehicle.Identity(),
            await _vehicle.Kind(),
            await _vehicle.Brand(),
            await _vehicle.Model(),
            await _vehicle.Price(),
            await _vehicle.Characteristics(),
            await _vehicle.Photos(),
            await _vehicle.Geo(),
            _parser);
    }
}