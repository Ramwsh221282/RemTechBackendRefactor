using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.LocationContext;

public sealed class LocationsRepository : ILocationsDataSource
{
    private readonly VehiclesServiceDbContext _context;

    public LocationsRepository(VehiclesServiceDbContext context) => _context = context;

    public async Task Add(Location location, CancellationToken ct = default)
    {
        await _context.Locations.AddAsync(location, ct);
        await _context.SaveChangesAsync(ct);
    }

    public Task<UniqueLocation> GetUnique(LocationAddress address, CancellationToken ct = default)
    {
        // TODO: Implement.
        throw new NotImplementedException();
    }
}
