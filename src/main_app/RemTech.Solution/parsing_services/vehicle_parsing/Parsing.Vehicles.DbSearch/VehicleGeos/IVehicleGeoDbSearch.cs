using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Parsing.Vehicles.DbSearch.VehicleGeos;

public interface IVehicleGeoDbSearch
{
    public Task<ParsedVehicleGeo> Search(string text);
}