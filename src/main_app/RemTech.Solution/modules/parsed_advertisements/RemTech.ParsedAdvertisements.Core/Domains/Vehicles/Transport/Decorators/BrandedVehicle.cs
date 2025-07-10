using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

public sealed class BrandedVehicle : VehicleEnvelope
{
    public BrandedVehicle(VehicleBrandEnvelope brand)
        : this(brand.Identify().ReadId(), brand.Identify().ReadText()) { }

    public BrandedVehicle(NotEmptyGuid id, NotEmptyString name, IVehicle origin)
        : this(new ExistingVehicleBrand(id, name), origin) { }

    public BrandedVehicle(NotEmptyGuid id, NotEmptyString name)
        : this(new ExistingVehicleBrand(id, name), new VehicleBlueprint()) { }

    public BrandedVehicle(NotEmptyString name, IVehicle origin)
        : this(new NewVehicleBrand(name), origin) { }

    public BrandedVehicle(NotEmptyString name)
        : this(new NewVehicleBrand(name), new VehicleBlueprint()) { }

    public BrandedVehicle(string? name, IVehicle origin)
        : this(new NotEmptyString(name), origin) { }

    public BrandedVehicle(string? name)
        : this(new NotEmptyString(name), new VehicleBlueprint()) { }

    public BrandedVehicle(Guid? id, string? name, IVehicle origin)
        : this(new NotEmptyGuid(id), new NotEmptyString(name), origin) { }

    public BrandedVehicle(Guid? id, string? name)
        : this(new NotEmptyGuid(id), new NotEmptyString(name), new VehicleBlueprint()) { }

    public BrandedVehicle(IVehicleBrand brand, IVehicle origin)
        : base(
            origin.Identity(),
            origin.Kind(),
            brand,
            origin.Location(),
            origin.Cost(),
            origin.TextInformation(),
            origin.Photos(),
            origin.Characteristics()
        ) { }
}
