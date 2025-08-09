using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Parsing.Vehicles.Common.ParsedVehicles;

public sealed class ParsedVehicle(
    IParsedVehicleBrandSource brandSource,
    IParsedVehicleModelSource modelSource,
    IParsedVehicleKindSource kindSource,
    IParsedVehicleIdentitySource identitySource,
    IParsedVehiclePriceSource priceSource,
    IKeyValuedCharacteristicsSource ctxSource,
    IParsedVehiclePhotos photosSource,
    IParsedVehicleUrlSource urlSource,
    IParsedVehicleGeoSource geoSource
) : IParsedVehicle
{
    public Task<ParsedVehicleIdentity> Identity() => identitySource.Read();

    public Task<ParsedVehicleBrand> Brand() => brandSource.Read();

    public Task<CharacteristicsDictionary> Characteristics() => ctxSource.Read();

    public Task<ParsedVehicleModel> Model() => modelSource.Read();

    public Task<ParsedVehicleKind> Kind() => kindSource.Read();

    public Task<UniqueParsedVehiclePhotos> Photos() => photosSource.Read();

    public Task<ParsedVehiclePrice> Price() => priceSource.Read();

    public Task<ParsedVehicleUrl> SourceUrl() => urlSource.Read();

    public Task<ParsedVehicleGeo.ParsedVehicleGeo> Geo() => geoSource.Read();

    public Task<string> Description()
    {
        return Task.FromResult(string.Empty);
    }
}
