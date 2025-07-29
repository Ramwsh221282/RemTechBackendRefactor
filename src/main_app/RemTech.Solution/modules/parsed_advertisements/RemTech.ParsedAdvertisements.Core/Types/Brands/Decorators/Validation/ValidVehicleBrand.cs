using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Validation;

public sealed class ValidVehicleBrand(VehicleBrand origin) : VehicleBrand(origin)
{
    private bool _identityPassed;
    protected override VehicleBrandIdentity Identity
    {
        get
        {
            VehicleBrandIdentity current = base.Identity;
            if (_identityPassed)
                return current;
            string name = current.ReadText();
            if (string.IsNullOrWhiteSpace(name))
                throw new ValueNotValidException("Название бренда");
            Guid id = current.ReadId();
            if (id == Guid.Empty)
                throw new ValueNotValidException("Идентификатор бренда");
            _identityPassed = true;
            return current;
        }
    }
}
