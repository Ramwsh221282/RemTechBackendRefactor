using RemTech.Vehicles.Module.Types.Brands;

namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal interface IVehicleKindsStorage
{
    Task<VehicleKind> Store(VehicleKind kind);
}
