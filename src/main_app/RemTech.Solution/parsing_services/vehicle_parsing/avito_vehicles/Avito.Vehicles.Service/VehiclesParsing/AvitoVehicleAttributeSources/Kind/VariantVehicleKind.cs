using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

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
            try
            {
                IParsedVehicleKindSource source = _sources.Dequeue();
                ParsedVehicleKind kind = await source.Read();
                return kind;
            }
            catch
            {
                // ignored
            }
        }

        throw new ArgumentException("Vehicle kind was not recognized in variant vehicle kind.");
    }
}
