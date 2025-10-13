using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Domain.LocationContext.Infrastructure.DataSource;

public interface ILocationsDataSource
{
    Task Add(Location location, CancellationToken ct = default);
    Task<UniqueLocation> GetUnique(LocationAddress address, CancellationToken ct = default);
}
