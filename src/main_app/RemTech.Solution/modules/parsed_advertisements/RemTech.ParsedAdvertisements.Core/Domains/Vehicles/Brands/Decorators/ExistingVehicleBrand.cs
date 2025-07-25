using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

public sealed class ExistingVehicleBrand : VehicleBrand
{
    public ExistingVehicleBrand(NotEmptyGuid id, NotEmptyString text)
        : base(
            new VehicleBrandIdentity(
                new VehicleBrandId(id),
                new NewVehicleBrand(text).Identify().ReadText()
            )
        ) { }

    public ExistingVehicleBrand(Guid? id, string? text)
        : this(new NotEmptyGuid(id), new NotEmptyString(text)) { }
}
