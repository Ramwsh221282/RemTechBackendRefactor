using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;

public sealed class NewCharacteristic : CharacteristicEnvelope
{
    public NewCharacteristic(NotEmptyString name)
        : base(
            new CharacteristicIdentity(
                new CharacteristicText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TrimmedText(
                                new TextWithoutPunctuation(
                                    new TextWithoutDigits(
                                        new TextWithOnlyRussianLetters(new RawText(name))
                                    )
                                )
                            )
                        )
                    ).Read()
                )
            )
        ) { }

    public NewCharacteristic(string? name)
        : this(new NotEmptyString(name)) { }
}
