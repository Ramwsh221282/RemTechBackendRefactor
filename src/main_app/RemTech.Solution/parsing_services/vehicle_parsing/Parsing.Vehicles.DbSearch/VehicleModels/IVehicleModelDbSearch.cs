using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

namespace Parsing.Vehicles.DbSearch.VehicleModels;

public interface IVehicleModelDbSearch
{
    Task<ParsedVehicleModel> Search(string text);
}