using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class VarietWebElementGeoSource : IParsedVehicleGeoSource
{
    private readonly Queue<IParsedVehicleGeoSource> _sources = [];

    public VarietWebElementGeoSource With(IParsedVehicleGeoSource source)
    {
        _sources.Enqueue(source);
        return this;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        while (_sources.Count > 0)
        {
            IParsedVehicleGeoSource source = _sources.Dequeue();
            ParsedVehicleGeo geo = await source.Read();
            if (geo)
                return geo;
        }

        return new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity());
    }
}