using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

public sealed class ExistingGeoLocation : GeoLocationEnvelope
{
    public ExistingGeoLocation(NotEmptyGuid id, NotEmptyString name)
        : base(
            new GeoLocationIdentity(
                new GeoLocationId(id),
                new NewGeoLocation(name).Identify().ReadText()
            )
        ) { }

    public ExistingGeoLocation(Guid? id, string? name)
        : this(new NotEmptyGuid(id), new NotEmptyString(name)) { }
}
