namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;

internal interface IVehicleBrandsStorage
{
    Task<VehicleBrand> Store(VehicleBrand brand);
}
