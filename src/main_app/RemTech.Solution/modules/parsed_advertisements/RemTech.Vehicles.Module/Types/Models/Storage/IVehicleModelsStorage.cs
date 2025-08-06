using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.Storage;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal interface IVehicleModelsStorage
{
    Task<VehicleModel> Store(VehicleModel vehicleModel);
}
