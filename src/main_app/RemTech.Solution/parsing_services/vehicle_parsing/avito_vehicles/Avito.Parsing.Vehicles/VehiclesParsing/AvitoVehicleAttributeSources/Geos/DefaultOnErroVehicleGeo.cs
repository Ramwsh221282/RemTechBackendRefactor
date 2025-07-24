using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class DefaultOnErroVehicleGeo : IParsedVehicleGeoSource
{
    private readonly IParsedVehicleGeoSource _origin;

    public DefaultOnErroVehicleGeo(IParsedVehicleGeoSource origin)
    {
        _origin = origin;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity());
        }
    }
}