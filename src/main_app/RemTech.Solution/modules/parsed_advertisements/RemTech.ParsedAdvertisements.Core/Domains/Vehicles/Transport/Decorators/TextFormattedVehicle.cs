using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class TextFormattedVehicle : VehicleEnvelope
{
    public TextFormattedVehicle(string? title, string? description)
        : this(title, description, new VehicleBlueprint()) { }

    public TextFormattedVehicle(string? title, string? description, IVehicle origin)
        : this(new NotEmptyString(title), new NotEmptyString(description), origin) { }

    public TextFormattedVehicle(NotEmptyString title, NotEmptyString description, IVehicle origin)
        : this(
            new VehicleText(
                new NotEmptyString(
                    new Text(
                        new CapitalizedFirstLetterText(new TrimmedText(new RawText(description)))
                    ).Read()
                ),
                new NotEmptyString(
                    new Text(
                        new CapitalizedFirstLetterText(new TrimmedText(new RawText(title)))
                    ).Read()
                )
            ),
            origin
        ) { }

    public TextFormattedVehicle(VehicleText text, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            origin.Brand(),
            origin.Location(),
            origin.Cost(),
            text,
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
