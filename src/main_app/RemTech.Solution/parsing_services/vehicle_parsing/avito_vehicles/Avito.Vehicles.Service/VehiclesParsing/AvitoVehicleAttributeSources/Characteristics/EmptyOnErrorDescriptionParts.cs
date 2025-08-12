namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class EmptyOnErrorDescriptionParts(IAvitoDescriptionParts origin)
    : IAvitoDescriptionParts
{
    public async Task<string> Read()
    {
        try
        {
            return await origin.Read();
        }
        catch
        {
            return string.Empty;
        }
    }
}
