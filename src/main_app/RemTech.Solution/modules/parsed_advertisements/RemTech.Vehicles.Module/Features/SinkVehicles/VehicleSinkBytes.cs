using System.Text.Json;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Brands;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;
using RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal sealed class VehicleSinkBytes : IVehicleJsonSink
{
    private readonly VehicleSinkMessage _message;

    public VehicleSinkBytes(byte[] bytes)
    {
        _message = JsonSerializer.Deserialize<VehicleSinkMessage>(bytes)!;
    }

    public VehicleKind Kind()
    {
        VehicleKindIdentity identity = new VehicleKindIdentity(
            new VehicleKindText(_message.Vehicle.Kind)
        );
        return new VehicleKind(identity);
    }

    public VehicleBrand Brand()
    {
        VehicleBrandIdentity brand = new VehicleBrandIdentity(
            new VehicleBrandText(_message.Vehicle.Brand)
        );
        return new VehicleBrand(brand);
    }

    public VehicleModel Model()
    {
        return new VehicleModel(
            new VehicleModelIdentity(Guid.NewGuid()),
            new VehicleModelName(_message.Vehicle.Model)
        );
    }

    public GeoLocation Location()
    {
        return new GeoLocation(
            new GeoLocationIdentity(
                new GeolocationText(_message.Vehicle.Geo),
                new GeolocationText(string.Empty)
            )
        );
    }

    public VehicleIdentity VehicleId()
    {
        return new VehicleIdentity(new VehicleId(_message.Vehicle.Id));
    }

    public IItemPrice VehiclePrice()
    {
        bool isNds = _message.Vehicle.IsNds;
        PriceValue value = new PriceValue(_message.Vehicle.Price);
        return isNds ? new ItemPriceWithNds(value) : new ItemPriceWithoutNds(value);
    }

    public VehiclePhotos VehiclePhotos()
    {
        return new VehiclePhotos(_message.Vehicle.Photos.Select(p => new VehiclePhoto(p.Source)));
    }

    public Vehicle Vehicle()
    {
        return new Vehicle(
            VehicleId(),
            VehiclePrice(),
            VehiclePhotos(),
            _message.Vehicle.SourceUrl,
            _message.Parser.ParserDomain,
            Description()
        );
    }

    public CharacteristicVeil[] Characteristics()
    {
        return _message
            .Vehicle.Characteristics.Select(c => new CharacteristicVeil(
                new NotEmptyString(c.Name),
                new NotEmptyString(c.Value)
            ))
            .ToArray();
    }

    public string ParserName()
    {
        return _message.Parser.ParserName;
    }

    public string ParserType()
    {
        return _message.Parser.ParserType;
    }

    public string LinkName()
    {
        return _message.Link.LinkName;
    }

    public string SourceUrl()
    {
        return _message.Vehicle.SourceUrl;
    }

    public string SourceDomain()
    {
        return _message.Parser.ParserDomain;
    }

    public string Description()
    {
        return _message.Vehicle.Description;
    }
}
