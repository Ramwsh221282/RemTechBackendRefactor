using System.Text.Json;
using RemTech.Vehicles.Module.Features.SinkVehicles.Types;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Features.SinkVehicles;

internal sealed class VehicleSinkBytes(byte[] bytes) : IVehicleJsonSink
{
    private readonly VehicleSinkMessage _message = JsonSerializer.Deserialize<VehicleSinkMessage>(
        bytes
    )!;

    public SinkedVehicleCategory Category()
    {
        return new SinkedVehicleCategory(_message.Vehicle.Kind, Guid.Empty);
    }

    public SinkedVehicleBrand Brand()
    {
        return new SinkedVehicleBrand(_message.Vehicle.Brand, Guid.Empty);
    }

    public SinkedVehicleModel Model()
    {
        return new SinkedVehicleModel(_message.Vehicle.Model, Guid.Empty);
    }

    public SinkedVehicleLocation Location()
    {
        return new SinkedVehicleLocation(
            _message.Vehicle.Geo,
            string.Empty,
            string.Empty,
            Guid.Empty
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
            Sentences()
        );
    }

    public IEnumerable<VehicleBodyCharacteristic> Characteristics()
    {
        return _message.Vehicle.Characteristics;
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

    public string Sentences()
    {
        return _message.Vehicle.Description;
    }
}
