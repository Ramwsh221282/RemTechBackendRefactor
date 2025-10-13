using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Domain.ModelContext;

public sealed class VehicleModel
{
    public VehicleModelId Id { get; }
    public VehicleModelName Name { get; private set; } = null!;
    public VehicleModelRating Rating { get; private set; }
    public VehicleModelVehicleCount VehiclesCount { get; private set; }

    private VehicleModel()
    {
        // ef core.
    }

    private VehicleModel(
        VehicleModelId id,
        VehicleModelName name,
        VehicleModelRating rating,
        VehicleModelVehicleCount vehiclesCount
    )
    {
        Id = id;
        Name = name;
        Rating = rating;
        VehiclesCount = vehiclesCount;
    }

    private static async Task<Result<VehicleModel>> Create(
        VehicleModelName name,
        IVehicleModelsDataSource dataSource,
        CancellationToken ct = default
    )
    {
        VehicleModelId id = new();
        VehicleModelRating rating = new();
        VehicleModelVehicleCount count = new();
        UniqueVehicleModel unique = await dataSource.GetUnique(name, ct);
        return Create(id, name, rating, count, unique);
    }

    private static Result<VehicleModel> Create(
        VehicleModelId id,
        VehicleModelName name,
        VehicleModelRating rating,
        VehicleModelVehicleCount vehiclesCount,
        UniqueVehicleModel unique
    )
    {
        VehicleModel model = new(id, name, rating, vehiclesCount);
        return unique.Unique(model);
    }
}
