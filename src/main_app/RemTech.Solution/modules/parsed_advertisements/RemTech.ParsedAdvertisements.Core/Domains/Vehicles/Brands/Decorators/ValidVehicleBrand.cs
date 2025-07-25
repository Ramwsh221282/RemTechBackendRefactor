using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;

public sealed class ValidVehicleBrand(VehicleBrand origin) : VehicleBrand(origin)
{
    public override BrandedVehicleModel Print(BrandedVehicleModel branded) =>
        !Identity ? throw new ValueNotValidException("ID или название бренда") : base.Print(branded);

    public override Vehicle Print(Vehicle vehicle) =>
        !Identity ? throw new ValueNotValidException("ID или название бренда") : base.Print(vehicle);

    public override PgVehicleBrandFromStoreCommand FromStoreCommand() =>
        !Identity ? throw new ValueNotValidException("ID или название бренда") : base.FromStoreCommand();

    public override PgVehicleBrandStoreCommand StoreCommand() =>
        !Identity ? throw new ValueNotValidException("ID или название бренда") : base.StoreCommand();

    public override VehicleBrandIdentity Identify() =>
        !Identity ? throw new ValueNotValidException("ID или название бренда") : base.Identify();
}