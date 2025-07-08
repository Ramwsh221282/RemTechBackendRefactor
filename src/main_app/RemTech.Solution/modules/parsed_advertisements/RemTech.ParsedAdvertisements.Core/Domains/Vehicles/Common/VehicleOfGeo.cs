using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;

public sealed class VehicleOfGeo
{
    private readonly ParsedTransport _transport;
    private readonly ParsedGeoLocation _geo;

    public VehicleOfGeo(ParsedTransport transport, ParsedGeoLocation geo)
    {
        _transport = transport;
        _geo = geo;
    }

    public static implicit operator ParsedTransport(VehicleOfGeo vog)
    {
        return vog._transport;
    }

    public static implicit operator ParsedGeoLocation(VehicleOfGeo vog)
    {
        return vog._geo;
    }
}
