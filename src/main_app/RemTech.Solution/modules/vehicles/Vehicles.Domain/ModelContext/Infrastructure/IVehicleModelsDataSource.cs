using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Domain.ModelContext.Infrastructure;

public interface IVehicleModelsDataSource
{
    Task Add(VehicleModel model, CancellationToken ct = default);
    Task<UniqueVehicleModel> GetUnique(VehicleModelName name, CancellationToken ct = default);
}
