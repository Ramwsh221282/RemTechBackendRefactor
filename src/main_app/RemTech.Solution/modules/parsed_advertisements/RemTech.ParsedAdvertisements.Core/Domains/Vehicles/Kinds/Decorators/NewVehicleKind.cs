using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;

public sealed class NewVehicleKind : VehicleKindEnvelope
{
    public NewVehicleKind(string? name)
        : this(new NotEmptyString(name)) { }

    public NewVehicleKind(NotEmptyString name)
        : base(
            new VehicleKindIdentity(
                new VehicleKindText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TrimmedText(new TextWithoutPunctuation(new RawText(name)))
                        )
                    ).Read()
                )
            )
        ) { }
}
