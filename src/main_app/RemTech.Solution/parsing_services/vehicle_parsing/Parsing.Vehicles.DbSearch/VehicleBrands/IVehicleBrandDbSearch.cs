using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

namespace Parsing.Vehicles.DbSearch.VehicleBrands;

public interface IVehicleBrandDbSearch
{
    Task<ParsedVehicleBrand> Search(string text);
}