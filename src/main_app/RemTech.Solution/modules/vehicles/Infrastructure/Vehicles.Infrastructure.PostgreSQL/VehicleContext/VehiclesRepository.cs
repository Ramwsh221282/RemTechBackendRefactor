using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;

namespace Vehicles.Infrastructure.PostgreSQL.VehicleContext;

public sealed class VehiclesRepository : IVehiclesDataSource
{
    private readonly VehiclesServiceDbContext _context;

    public VehiclesRepository(VehiclesServiceDbContext context) => _context = context;

    public async Task<Vehicle> Add(Vehicle vehicle, CancellationToken ct = default)
    {
        await _context.Vehicles.AddAsync(vehicle, ct);
        await _context.SaveChangesAsync(ct);
        return vehicle;
    }
}
