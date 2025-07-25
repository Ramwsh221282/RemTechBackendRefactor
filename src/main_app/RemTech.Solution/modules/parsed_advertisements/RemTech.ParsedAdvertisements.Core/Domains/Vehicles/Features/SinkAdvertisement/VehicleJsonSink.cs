using System.Text;
using RemTech.Core.Shared.Primitives;
using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.SinkAdvertisement;

public sealed class VehicleJsonSink : IVehicleJsonSink
{
    private readonly DesJsonSource _source;
    public VehicleJsonSink(string json) => _source = new DesJsonSource(json);

    public VehicleJsonSink(ReadOnlyMemory<byte> bytes)
    {
        _source = new DesJsonSource(Encoding.UTF8.GetString(bytes.Span));
    }

    public VehicleKind Kind()
    {
        return new NewVehicleKind(
            new NotEmptyString(
                new DesJsonString(_source.ParserElement("kind")))
        );
    }

    public VehicleBrand Brand()
    {
        return new NewVehicleBrand(
            new NotEmptyString(
                new DesJsonString(_source.ParserElement("brand"))));
    }

    public VehicleModel Model()
    {
        return new VehicleModel(
            new VehicleModelIdentity(Guid.NewGuid()),
            new VehicleModelName(
                new DesJsonString(_source.ParserElement("model"))));
    }

    public GeoLocation Location()
    {
        return new NewGeoLocation(
            new DesJsonString(_source.ParserElement("region")),
            new NotEmptyString(string.Empty));
    }

    public VehicleIdentity VehicleId()
    {
        return new VehicleIdentity(
            new VehicleId(
                new DesJsonString(_source.ParserElement("id"))));
    }

    public IItemPrice VehiclePrice()
    {
        PriceValue priceValue = new(
            new PositiveLong(
                new DesJsonLong(_source.ParserElement("price"))));
        bool isNds = new DesJsonBoolean(_source.ParserElement("isNds"));
        return isNds ? new ItemPriceWithNds(priceValue) : new ItemPriceWithoutNds(priceValue);
    }

    public VehiclePhotos VehiclePhotos()
    {
        return new VehiclePhotos(
            new DesJsonArray(_source.ParserElement("photos"))
                .MapEach(el => new VehiclePhoto(
            new DesJsonString(
                new DesJsonElement(el.Property("source"))))));
    }
    
    public Vehicle Vehicle()
    {
        return new Vehicle(
            VehicleId(),
            VehiclePrice(),
            VehiclePhotos());
    }

    public CharacteristicVeil[] Characteristics()
    {
        return new DesJsonArray(_source.ParserElement("characteristics"))
            .MapEach(el => new CharacteristicVeil(
            new NotEmptyString(new DesJsonString(el.Property("name"))),
            new NotEmptyString(new DesJsonString(el.Property("value")))
        ));
    }

    public string ParserName()
    {
        return new DesJsonString(_source.ParserElement("parserName"));
    }

    public string ParserType()
    {
        return new DesJsonString(_source.ParserElement("parserType"));
    }

    public string LinkName()
    {
        return new DesJsonString(_source.ParserElement("linkName"));
    }

    public Characteristic[] AddedCharacteristics()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _source.Dispose();
    }
    
    // public ParsedVehicleJson Json()
    // {
    //     string json = new PlainSerJson()
    //         .With(new StringSerJson("id", _identity))
    //         .With(new StringSerJson("kind", _kind))
    //         .With(new StringSerJson("brand", _brand))
    //         .With(new StringSerJson("model", _model))
    //         .With(new LongSerJson("price", _price))
    //         .With(new BooleanSerJson("isNds", _price.IsNds()))
    //         .With(new StringSerJson("region", _geo.Region()))
    //         .With(new StringSerJson("city", _geo.City()))
    //         .With(new StringSerJson("parserName", _parser.ParserName()))
    //         .With(new StringSerJson("parserType", _parser.ParserType()))
    //         .With(new StringSerJson("linkName", _parser.LinkName()))
    //         .With(new ObjectsArraySerJson<VehicleCharacteristic>("characteristics", _characteristics.Read())
    //             .ForEach(l =>
    //                 new PlainSerJson()
    //                     .With(new StringSerJson("name", l.Name()))
    //                     .With(new StringSerJson("value", l.Value()))))
    //         .With(new ObjectsArraySerJson<string>("photos", _photos.Read())
    //             .ForEach(p => 
    //                 new PlainSerJson()
    //                     .With(new StringSerJson("source", p))))
    //         .Read();
    //     return new ParsedVehicleJson(json);
    // }
}