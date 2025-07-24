using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;
using RemTech.Json.Library.Serialization.Primitives;

namespace Parsing.Vehicles.Common.Json;

public sealed class ParsedVehicleInfo
{
    private readonly ParsedVehicleIdentity _identity;
    private readonly ParsedVehicleKind _kind;
    private readonly ParsedVehicleBrand _brand;
    private readonly ParsedVehicleModel _model;
    private readonly ParsedVehiclePrice _price;
    private readonly CharacteristicsDictionary _characteristics;
    private readonly UniqueParsedVehiclePhotos _photos;
    private readonly ParsedVehicleGeo _geo;
    private readonly ParsedVehicleParser _parser;
    public ParsedVehicleInfo(
        ParsedVehicleIdentity identity,
        ParsedVehicleKind kind,
        ParsedVehicleBrand brand,
        ParsedVehicleModel model,
        ParsedVehiclePrice price,
        CharacteristicsDictionary characteristics,
        UniqueParsedVehiclePhotos photos,
        ParsedVehicleGeo geo,
        ParsedVehicleParser parser
    )
    {
        _identity = identity;
        _kind = kind;
        _brand = brand;
        _model = model;
        _price = price;
        _characteristics = characteristics;
        _photos = photos;
        _geo = geo;
        _parser = parser;
    }

    public ParsedVehicleJson Json()
    {
        string json = new PlainSerJson()
            .With(new StringSerJson("id", _identity))
            .With(new StringSerJson("kind", _kind))
            .With(new StringSerJson("brand", _brand))
            .With(new StringSerJson("model", _model))
            .With(new LongSerJson("price", _price))
            .With(new BooleanSerJson("isNds", _price.IsNds()))
            .With(new StringSerJson("region", _geo.Region()))
            .With(new StringSerJson("city", _geo.City()))
            .With(new StringSerJson("parserName", _parser.ParserName()))
            .With(new StringSerJson("parserType", _parser.ParserType()))
            .With(new StringSerJson("linkName", _parser.LinkName()))
            .With(new ObjectsArraySerJson<VehicleCharacteristic>("characteristics", _characteristics.Read())
                .ForEach(l =>
                    new PlainSerJson()
                        .With(new StringSerJson("name", l.Name()))
                        .With(new StringSerJson("value", l.Value()))))
            .With(new ObjectsArraySerJson<string>("photos", _photos.Read())
                .ForEach(p => 
                    new PlainSerJson()
                        .With(new StringSerJson("source", p))))
            .Read();
        return new ParsedVehicleJson(json);
    }
}