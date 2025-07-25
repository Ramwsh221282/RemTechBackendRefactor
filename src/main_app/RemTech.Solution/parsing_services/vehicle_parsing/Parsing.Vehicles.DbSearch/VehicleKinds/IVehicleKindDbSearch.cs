using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public interface IVehicleKindDbSearch
{
    Task<ParsedVehicleKind> Search(string text);
}