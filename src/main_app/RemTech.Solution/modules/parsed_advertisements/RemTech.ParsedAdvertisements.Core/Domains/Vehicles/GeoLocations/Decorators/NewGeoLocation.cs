using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;

public sealed class NewGeoLocation : GeoLocationEnvelope
{
    public NewGeoLocation(NotEmptyString name)
        : base(
            new GeoLocationIdentity(
                new GeolocationText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TrimmedText(new TextWithoutPunctuation(new RawText(name)))
                        )
                    ).Read()
                )
            )
        ) { }

    public NewGeoLocation(string? name)
        : this(new NotEmptyString(name)) { }
}
