namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal interface IVehicleBrandsStorage
{
    Task<VehicleBrand> Store(VehicleBrand brand);
}
