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

    public async Task<Location> Add(Location location, CancellationToken ct = default)
    {
        await _context.Locations.AddAsync(location, ct);
        await _context.SaveChangesAsync(ct);
        return location;
    }

    public async Task<Location> GetOrSave(Location location, CancellationToken ct)
    {
        Location? relevant = await GetRelevantByAddress(location.Address, ct);
        return relevant ?? await Add(location, ct);
    }

    private async Task<Location?> GetRelevantByAddress(
        LocationAddress address,
        CancellationToken ct
    )
    {
        Pgvector.Vector vector = address.GenerateVector(_generator);
        return await _context
            .Locations.FromSqlInterpolated(
                $@"
                SELECT
                id,
                rating,
                vehicles_count,
                address,
                embedding
                FROM vehicles_module.locations
                ORDER BY embedding <=> {vector}
                LIMIT 1
                "
            )
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
