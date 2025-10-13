using Microsoft.EntityFrameworkCore;
using RemTech.Infrastructure.PostgreSQL.Vector;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Infrastructure.PostgreSQL.LocationContext;

public sealed class LocationsRepository : ILocationsDataSource
{
    private readonly VehiclesServiceDbContext _context;
    private readonly IEmbeddingGenerator _generator;

    public LocationsRepository(VehiclesServiceDbContext context, IEmbeddingGenerator generator)
    {
        _context = context;
        _generator = generator;
    }

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

    public async Task<Location> GetOrSave(LocationAddress address, CancellationToken ct)
    {
        Pgvector.Vector vector = address.GenerateVector(_generator);
        return await _context
            .Locations.FromSqlInterpolated(
                $@"
                SELECT
                id,
                rating,
                vehicles_count,
                address
                FROM locations
                ORDER BY embedding <=> {vector}
                LIMIT 1
                "
            )
            .FirstAsync(cancellationToken: ct);
    }
}
