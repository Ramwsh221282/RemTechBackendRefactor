using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Parsing.Vehicles.Common.ParsedVehicles;

public interface IParsedVehicle
{
    Task<ParsedVehicleIdentity> Identity();
    Task<ParsedVehicleBrand> Brand();
    Task<KeyValueVehicleCharacteristics> Characteristics();
    Task<ParsedVehicleModel> Model();
    Task<ParsedVehicleKind> Kind();
    Task<UniqueParsedVehiclePhotos> Photos();
    Task<ParsedVehiclePrice> Price();
    
    Task<ParsedVehicleUrl> SourceUrl();
}