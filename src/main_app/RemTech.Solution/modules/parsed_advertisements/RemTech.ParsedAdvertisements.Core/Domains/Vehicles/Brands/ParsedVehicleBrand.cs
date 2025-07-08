using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public sealed class ParsedVehicleBrand
{
    private readonly ParsedVehicleBrandIdentity _identity;
    private VehicleOfBrand[] _vehicles;

    public ParsedVehicleBrand(ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrandId id, ParsedVehicleBrandText text)
        : this(new ParsedVehicleBrandIdentity(id, text), []) { }

    public ParsedVehicleBrand(ParsedVehicleBrandIdentity identity, VehicleOfBrand[] vehicles)
    {
        _vehicles = vehicles;
        _identity = identity;
    }

    public VehicleOfBrand PutBrandMark(ParsedTransport transport)
    {
        VehicleOfBrand vehicle = new(transport, this);
        _vehicles = [.. _vehicles, vehicle];
        return vehicle;
    }

    public ParsedVehicleBrandIdentity Identify() => _identity;
}
