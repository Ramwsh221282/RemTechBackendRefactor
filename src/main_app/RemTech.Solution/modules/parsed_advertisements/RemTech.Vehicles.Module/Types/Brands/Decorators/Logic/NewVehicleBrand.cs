﻿using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;

public sealed class NewVehicleBrand : VehicleBrand
{
    public NewVehicleBrand(NotEmptyString name)
        : base(
            new VehicleBrandIdentity(
                new VehicleBrandText(
                    new Text(
                        new CapitalizedFirstLetterText(
                            new TextWithOnlyOneWhiteSpaces(
                                new TrimmedText(new TextWithoutPunctuation(new RawText(name)))
                            )
                        )
                    ).Read()
                )
            )
        ) { }

    public NewVehicleBrand(string? name)
        : this(new NotEmptyString(name)) { }
}
