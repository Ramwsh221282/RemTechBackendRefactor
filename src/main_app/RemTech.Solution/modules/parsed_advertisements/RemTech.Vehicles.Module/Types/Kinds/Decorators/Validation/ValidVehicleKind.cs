﻿using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds.Decorators.Validation;

public sealed class ValidVehicleKind(VehicleKind origin) : VehicleKind(origin)
{
    private bool _identityPassed;
    protected override VehicleKindIdentity Identity
    {
        get
        {
            VehicleKindIdentity identity = base.Identity;
            if (_identityPassed)
                return identity;
            Guid id = identity.ReadId();
            string name = identity.ReadText();
            if (id == Guid.Empty)
                throw new ValueNotValidException("Идентификатор типа техники");
            if (string.IsNullOrWhiteSpace(name))
                throw new ValueNotValidException("Название типа техники");
            _identityPassed = true;
            return identity;
        }
    }
}
