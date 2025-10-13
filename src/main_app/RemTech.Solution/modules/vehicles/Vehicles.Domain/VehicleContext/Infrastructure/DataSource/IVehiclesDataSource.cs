namespace Vehicles.Domain.VehicleContext.Infrastructure.DataSource;

public interface IVehiclesDataSource
{
    Task Add(Vehicle vehicle, CancellationToken ct = default);
}
