using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;

public interface IVehicleBrand
{
    VehicleBrandIdentity Identify();
    BrandedVehicleModel Print(BrandedVehicleModel branded);
    Vehicle Print(Vehicle vehicle);
    PgVehicleBrandFromStoreCommand FromStoreCommand();
    PgVehicleBrandStoreCommand StoreCommand();
}