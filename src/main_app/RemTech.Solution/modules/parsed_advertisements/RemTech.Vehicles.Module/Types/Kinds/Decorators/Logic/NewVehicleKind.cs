﻿using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;

public sealed class NewVehicleKind : VehicleKind
{
    public NewVehicleKind(string? name)
        : this(new NotEmptyString(name)) { }

    public NewVehicleKind(NotEmptyString name)
        : base(
            new VehicleKindIdentity(
                new VehicleKindText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TrimmedText(
                                new TextWithOnlyOneWhiteSpaces(
                                    new TextWithoutDigits(
                                        new TextWithOnlyRussianLetters(
                                            new TextWithoutPunctuation(new RawText(name))
                                        )
                                    )
                                )
                            )
                        )
                    ).Read()
                )
            )
        ) { }
}
