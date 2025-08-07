using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class DefaultOnErroVehicleGeo(IParsedVehicleGeoSource origin)
    : IParsedVehicleGeoSource
{
    public async Task<ParsedVehicleGeo> Read()
    {
        try
        {
            return await origin.Read();
        }
        catch
        {
            return new ParsedVehicleGeo(null);
        }
    }
}
