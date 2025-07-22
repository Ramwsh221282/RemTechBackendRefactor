using Parsing.Vehicles.Common.ParsedVehicles;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleEnvelope : IParsedVehicle
{
    private readonly ParsedVehicleIdentity _identity;
    private readonly ParsedVehicleBrand _brand;
    private readonly ParsedVehicleKind _kind;
    private readonly ParsedVehicleModel _model;
    private readonly KeyValueVehicleCharacteristics _ctx;
    private readonly UniqueParsedVehiclePhotos _photos;
    private readonly ParsedVehiclePrice _price;
    private readonly ParsedVehicleUrl _urlSource;

    public AvitoVehicleEnvelope()
    {
        _identity = new ParsedVehicleIdentity(new NotEmptyString(string.Empty));
        _brand = new ParsedVehicleBrand(new NotEmptyString(string.Empty));
        _kind = new ParsedVehicleKind(new NotEmptyString(string.Empty));
        _model = new ParsedVehicleModel(new NotEmptyString(string.Empty));
        _ctx = new KeyValueVehicleCharacteristics();
        _photos = new UniqueParsedVehiclePhotos();
        _price = new ParsedVehiclePrice(-1, string.Empty);
        _urlSource = new ParsedVehicleUrl(new NotEmptyString(string.Empty));
    }

    public AvitoVehicleEnvelope(AvitoVehicleEnvelope origin)
    {
        _identity = origin._identity;
        _brand = origin._brand;
        _kind = origin._kind;
        _model = origin._model;
        _ctx = origin._ctx;
        _photos = origin._photos;
        _price = origin._price;
        _urlSource = origin._urlSource;
    }

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        ParsedVehiclePrice price)
        : this(origin) => _price = price;

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        UniqueParsedVehiclePhotos photos)
        : this(origin) => _photos = photos;
    
    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        ParsedVehicleIdentity identity)
        : this(origin)
        => _identity = identity;

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        ParsedVehicleBrand brand)
        : this(origin) =>
        _brand = brand;

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        ParsedVehicleKind kind)
        : this(origin) => _kind = kind;

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        ParsedVehicleModel model) 
        : this(origin)
        => _model = model;

    public AvitoVehicleEnvelope(
        AvitoVehicleEnvelope origin,
        KeyValueVehicleCharacteristics ctx)
        : this(origin) => _ctx = ctx;

    public AvitoVehicleEnvelope(AvitoVehicleEnvelope origin, ParsedVehicleUrl url)
        : this(origin) => _urlSource = url;
    

    public Task<ParsedVehicleIdentity> Identity() =>
        Task.FromResult(_identity);

    public Task<ParsedVehicleBrand> Brand() =>
        Task.FromResult(_brand);

    public Task<KeyValueVehicleCharacteristics> Characteristics() =>
        Task.FromResult(_ctx);

    public Task<ParsedVehicleModel> Model() =>
        Task.FromResult(_model);

    public Task<ParsedVehicleKind> Kind() =>
        Task.FromResult(_kind);

    public Task<UniqueParsedVehiclePhotos> Photos() =>
        Task.FromResult(_photos);

    public Task<ParsedVehiclePrice> Price() =>
        Task.FromResult(_price);

    public Task<ParsedVehicleUrl> SourceUrl() =>
        Task.FromResult(_urlSource);
}