using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public abstract class VehicleBrand : IVehicleBrand
{
    private readonly VehicleBrandIdentity _identity;
    
    public VehicleBrand() => _identity = new VehicleBrandIdentity();
    public VehicleBrand(VehicleBrandIdentity identity) => _identity = identity;
    public VehicleBrandIdentity Identify() => _identity;
    
    public BrandedVehicleModel Print(BrandedVehicleModel branded) => new(branded, _identity);
    
    public Vehicle Print(Vehicle vehicle) => new(vehicle, this);

    public PgVehicleBrandFromStoreCommand FromStoreCommand() => new(_identity);
    public PgVehicleBrandStoreCommand StoreCommand() => new(_identity);
}
