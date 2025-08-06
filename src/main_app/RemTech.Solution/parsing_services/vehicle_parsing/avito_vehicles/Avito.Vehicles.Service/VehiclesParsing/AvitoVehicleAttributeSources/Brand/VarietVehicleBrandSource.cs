using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class VarietVehicleBrandSource : IParsedVehicleBrandSource
{
    private readonly Queue<IParsedVehicleBrandSource> _sources = [];

    public VarietVehicleBrandSource With(IParsedVehicleBrandSource other)
    {
        _sources.Enqueue(other);
        return this;
    }

    public async Task<ParsedVehicleBrand> Read()
    {
        while (_sources.Count > 0)
        {
            try
            {
                IParsedVehicleBrandSource source = _sources.Dequeue();
                return await source.Read();
            }
            catch
            {
                // ignored
            }
        }

        throw new ArgumentException("Unable to specify vehicle brand.");
    }
}
