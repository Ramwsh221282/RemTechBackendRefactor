namespace Vehicles.Domain.LocationContext.Infrastructure.DataSource;

public interface ILocationsDataSource
{
    Task<Location> Add(Location location, CancellationToken ct = default);
    Task<Location> GetOrSave(Location location, CancellationToken ct);
}
