using RemTech.Result.Pattern;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.LocationContext;

public sealed class Location
{
    public LocationId Id { get; }
    public LocationAddress Address { get; }
    public LocationRating Rating { get; }
    public LocationVehiclesCount VehicleCount { get; private set; }

    private Location(
        LocationId id,
        LocationAddress address,
        LocationRating rating,
        LocationVehiclesCount vehiclesCount
    )
    {
        Id = id;
        Address = address;
        Rating = rating;
        VehicleCount = vehiclesCount;
    }

    public static Result<Location> Create(
        LocationId id,
        LocationAddress address,
        LocationRating rating,
        LocationVehiclesCount vehiclesCount,
        UniqueLocation unique
    )
    {
        Location location = new(id, address, rating, vehiclesCount);
        return unique.Unique(location);
    }

    public static async Task<Result<Location>> Create(
        LocationAddress address,
        ILocationsDataSource dataSource,
        CancellationToken ct = default
    )
    {
        LocationId id = new();
        LocationRating rating = new();
        LocationVehiclesCount vehiclesCount = new();
        UniqueLocation unique = await dataSource.GetUnique(address, ct);
        return Create(id, address, rating, vehiclesCount, unique);
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle.LocationId != Id)
            return;
        VehicleCount = VehicleCount.Increase();
    }
}
