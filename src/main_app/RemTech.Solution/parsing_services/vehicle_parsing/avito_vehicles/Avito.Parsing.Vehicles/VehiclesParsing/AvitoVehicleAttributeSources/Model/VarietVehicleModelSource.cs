using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.DbSearch.VehicleModels;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class VarietVehicleModelSource : IParsedVehicleModelSource
{
    private readonly Queue<IParsedVehicleModelSource> _sources = [];

    public VarietVehicleModelSource With(IParsedVehicleModelSource other)
    {
        _sources.Enqueue(other);
        return this;
    }

    public async Task<ParsedVehicleModel> Read()
    {
        while (_sources.Count > 0)
        {
            try
            {
                IParsedVehicleModelSource source = _sources.Dequeue();
                return await source.Read();
            }
            catch
            {
                // ignored
            }
        }

        throw new ArgumentException("Unable to specify vehicle model.");
    }
}
