using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Common.ParsedVehicles;

public sealed class ParsedVehicle : IParsedVehicle
{
    private readonly IParsedVehicleBrandSource _brandSource;
    private readonly IParsedVehicleModelSource _modelSource;
    private readonly IParsedVehicleKindSource _kindSource;
    private readonly IParsedVehicleIdentitySource _identitySource;
    private readonly IParsedVehiclePriceSource _priceSource;
    private readonly IKeyValuedCharacteristicsSource _ctxSource;
    private readonly IParsedVehiclePhotos _photosSource;
    private readonly IParsedVehicleUrlSource _urlSource;
    private readonly IParsedVehicleGeoSource _geoSource;

    
    public ParsedVehicle(
        IParsedVehicleBrandSource brandSource,
        IParsedVehicleModelSource modelSource,
        IParsedVehicleKindSource kindSource,
        IParsedVehicleIdentitySource identitySource,
        IParsedVehiclePriceSource priceSource,
        IKeyValuedCharacteristicsSource ctxSource,
        IParsedVehiclePhotos photosSource,
        IParsedVehicleUrlSource urlSource,
        IParsedVehicleGeoSource geoSource)
    {
        _brandSource = brandSource;
        _modelSource = modelSource;
        _kindSource = kindSource;
        _identitySource = identitySource;
        _priceSource = priceSource;
        _ctxSource = ctxSource;
        _photosSource = photosSource;
        _urlSource = urlSource;
        _geoSource = geoSource;
    }
    
    public Task<ParsedVehicleIdentity> Identity() => _identitySource.Read();

    public Task<ParsedVehicleBrand> Brand() => _brandSource.Read();

    public Task<CharacteristicsDictionary> Characteristics() => _ctxSource.Read();

    public Task<ParsedVehicleModel> Model() => _modelSource.Read();

    public Task<ParsedVehicleKind> Kind() => _kindSource.Read();

    public Task<UniqueParsedVehiclePhotos> Photos() => _photosSource.Read();

    public Task<ParsedVehiclePrice> Price() => _priceSource.Read();

    public Task<ParsedVehicleUrl> SourceUrl() => _urlSource.Read();
    public Task<ParsedVehicleGeo.ParsedVehicleGeo> Geo() => _geoSource.Read();
}