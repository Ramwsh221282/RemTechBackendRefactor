using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

public sealed class ExistingGeoLocation : GeoLocationEnvelope
{
    public ExistingGeoLocation(NotEmptyGuid id, NotEmptyString name, NotEmptyString kind)
        : base(
            new GeoLocationIdentity(
                new GeoLocationId(id),
                new NewGeoLocation(name, kind).Identify().ReadText(),
                new NewGeoLocation(name, kind).Identify().ReadText()
            )
        ) { }

    public ExistingGeoLocation(Guid? id, string? name, string? kind)
        : this(new NotEmptyGuid(id), new NotEmptyString(name), new NotEmptyString(kind)) { }
}
