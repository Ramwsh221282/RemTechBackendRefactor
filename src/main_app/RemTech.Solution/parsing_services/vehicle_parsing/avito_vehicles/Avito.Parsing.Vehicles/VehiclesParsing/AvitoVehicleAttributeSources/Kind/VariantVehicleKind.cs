using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class VariantVehicleKind : IParsedVehicleKindSource
{
    private readonly Queue<IParsedVehicleKindSource> _sources = [];

    public VariantVehicleKind With(IParsedVehicleKindSource source)
    {
        _sources.Enqueue(source);
        return this;
    }
    
    public async Task<ParsedVehicleKind> Read()
    {
        while (_sources.Count > 0)
        {
            IParsedVehicleKindSource source = _sources.Dequeue();
            ParsedVehicleKind kind = await source.Read();
            if (kind)
                return kind;
        }

        throw new ArgumentException("Vehicle kind was not recognized in variant vehicle kind.");
    }
}