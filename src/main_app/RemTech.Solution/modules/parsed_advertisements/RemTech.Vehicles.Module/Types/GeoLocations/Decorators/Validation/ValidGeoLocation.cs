using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Validation;

public sealed class ValidGeoLocation(GeoLocation origin) : GeoLocation(origin)
{
    private bool _identityPassed;
    protected override GeoLocationIdentity Identity
    {
        get
        {
            GeoLocationIdentity identity = base.Identity;
            if (_identityPassed)
                return identity;
            Guid id = identity.ReadId();
            string name = identity.ReadText();
            if (id == Guid.Empty)
                throw new ValueNotValidException("Идентификатор локации");
            if (string.IsNullOrWhiteSpace(name))
                throw new ValueNotValidException("Название локации");
            _identityPassed = true;
            return identity;
        }
    }
}
