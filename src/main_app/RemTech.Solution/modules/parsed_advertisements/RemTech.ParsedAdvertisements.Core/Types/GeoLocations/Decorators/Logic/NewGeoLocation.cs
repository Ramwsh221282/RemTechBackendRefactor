using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;

public sealed class NewGeoLocation : GeoLocation
{
    public NewGeoLocation(NotEmptyString name, NotEmptyString kind)
        : base(
            new GeoLocationIdentity(
                new GeolocationText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TrimmedText(
                                new TextWithOnlyOneWhiteSpaces(
                                    new TextWithOnlyRussianLetters(
                                        new TextWithoutDigits(
                                            new TextWithoutPunctuation(new RawText(name))
                                        )
                                    )
                                )
                            )
                        )
                    ).Read()
                ),
                new GeolocationText(
                    new Text(
                        new TrimmedText(
                            new TextWithOnlyOneWhiteSpaces(
                                new TextWithOnlyRussianLetters(
                                    new TextWithoutDigits(
                                        new TextWithoutPunctuation(new RawText(kind))
                                    )
                                )
                            )
                        )
                    ).Read()
                )
            )
        ) { }

    public NewGeoLocation(string? name, string? kind)
        : this(new NotEmptyString(name), new NotEmptyString(kind)) { }
}
