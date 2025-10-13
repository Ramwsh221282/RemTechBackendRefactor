namespace Vehicles.Domain.VehicleContext.Infrastructure.DataSource;

public interface IVehiclesDataSource
{
    Task<Vehicle> Add(Vehicle vehicle, CancellationToken ct = default);
}
