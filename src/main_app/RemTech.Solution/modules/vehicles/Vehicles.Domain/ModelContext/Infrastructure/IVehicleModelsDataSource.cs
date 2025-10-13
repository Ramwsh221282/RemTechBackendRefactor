namespace Vehicles.Domain.ModelContext.Infrastructure;

public interface IVehicleModelsDataSource
{
    Task<VehicleModel> Add(VehicleModel model, CancellationToken ct = default);
    Task<VehicleModel> GetOrSave(VehicleModel model, CancellationToken ct);
}
