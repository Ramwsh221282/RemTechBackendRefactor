namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class EmptyOnErrorDescriptionParts : IAvitoDescriptionParts
{
    private readonly IAvitoDescriptionParts _origin;

    public EmptyOnErrorDescriptionParts(IAvitoDescriptionParts origin)
    {
        _origin = origin;
    }

    public async Task<string[]> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return [];
        }
    }
}
