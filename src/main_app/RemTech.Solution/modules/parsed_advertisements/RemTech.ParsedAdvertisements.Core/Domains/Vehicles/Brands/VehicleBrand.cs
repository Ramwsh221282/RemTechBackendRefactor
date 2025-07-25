using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public class VehicleBrand : IVehicleBrand
{
    protected readonly VehicleBrandIdentity Identity;
    
    public VehicleBrand(VehicleBrandIdentity identity) => Identity = identity;

    public VehicleBrand(VehicleBrand origin) => Identity = origin.Identity;

    public virtual VehicleBrandIdentity Identify() => Identity;

    public virtual BrandedVehicleModel Print(BrandedVehicleModel branded) => new(branded, Identity);

    public virtual Vehicle Print(Vehicle vehicle) => new(vehicle, this);

    public virtual PgVehicleBrandFromStoreCommand FromStoreCommand() => new(Identity);

    public virtual PgVehicleBrandStoreCommand StoreCommand() => new(Identity);
}
