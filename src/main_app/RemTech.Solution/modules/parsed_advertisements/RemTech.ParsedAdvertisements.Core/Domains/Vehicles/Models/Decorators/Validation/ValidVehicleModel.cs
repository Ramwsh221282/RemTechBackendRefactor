using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Validation;

public class ValidVehicleModel(VehicleModel origin) : VehicleModel(origin)
{
    private bool _identityPassed;
    private bool _namePassed;
    
    protected override VehicleModelIdentity Identity
    {
        get
        {
            VehicleModelIdentity identity = base.Identity;
            if (_identityPassed) return identity;
            Guid id = identity;
            if (id == Guid.Empty)
                throw new ValueNotValidException("Идентификатор техники пустой.");
            _identityPassed = true;
            return identity;
        }
    }

    protected override VehicleModelName Name
    {
        get
        {
            VehicleModelName name = base.Name;
            if (_namePassed) return name;
            string stringName = name;
            if (string.IsNullOrWhiteSpace(name))
                throw new ValueNotValidException("Название модели пустое.");
            _namePassed = true;
            return name;
        }
    }
}