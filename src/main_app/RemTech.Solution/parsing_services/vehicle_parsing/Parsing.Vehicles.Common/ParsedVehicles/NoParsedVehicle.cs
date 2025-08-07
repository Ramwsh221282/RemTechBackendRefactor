using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Parsing.Vehicles.Common.ParsedVehicles;

public sealed class NoParsedVehicle : IParsedVehicle
{
    public Task<ParsedVehicleIdentity> Identity() =>
        Task.FromResult(new ParsedVehicleIdentity(string.Empty));

    public Task<ParsedVehicleBrand> Brand() =>
        Task.FromResult(new ParsedVehicleBrand(string.Empty));

    public Task<CharacteristicsDictionary> Characteristics() =>
        Task.FromResult(new CharacteristicsDictionary());

    public Task<ParsedVehicleModel> Model() =>
        Task.FromResult(new ParsedVehicleModel(string.Empty));

    public Task<ParsedVehicleKind> Kind() => Task.FromResult(new ParsedVehicleKind(string.Empty));

    public Task<UniqueParsedVehiclePhotos> Photos() =>
        Task.FromResult(new UniqueParsedVehiclePhotos());

    public Task<ParsedVehiclePrice> Price() =>
        Task.FromResult(new ParsedVehiclePrice(-1, string.Empty));

    public Task<ParsedVehicleUrl> SourceUrl() => Task.FromResult(new ParsedVehicleUrl(null));

    public Task<ParsedVehicleGeo.ParsedVehicleGeo> Geo() =>
        Task.FromResult(new ParsedVehicleGeo.ParsedVehicleGeo(null));
}
