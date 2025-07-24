namespace Parsing.Vehicles.Common.ParsedVehicles;

public sealed class ValidatingParsedVehicle
{
    private readonly IParsedVehicle _vehicle;

    public ValidatingParsedVehicle(IParsedVehicle vehicle)
    {
        _vehicle = vehicle;
    }

    public async Task<bool> IsValid()
    {
        if (!await _vehicle.Identity())
            return false;
        if (!await _vehicle.Brand())
            return false;
        if (!await _vehicle.Model())
            return false;
        if (!await _vehicle.Characteristics())
            return false;
        if (!await _vehicle.Geo())
            return false;
        if (!await _vehicle.Price())
            return false;
        if (!await _vehicle.Kind())
            return false;
        if (!await _vehicle.SourceUrl())
            return false;
        return true;
    }
}