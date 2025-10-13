using RemTech.Core.Shared.Enumerables;
using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehiclePhotosCollection
{
    public IReadOnlyList<VehiclePhoto> Photos { get; }

    public VehiclePhotosCollection(IEnumerable<VehiclePhoto> photos)
    {
        Photos = [.. photos];
    }

    public static Result<VehiclePhotosCollection> Create(IEnumerable<string> photos)
    {
        IEnumerable<Result<VehiclePhoto>> results = photos.Select(VehiclePhoto.Create);
        Result<VehiclePhoto>? failure = results.FirstOrDefault(r => r.IsFailure);
        return failure?.Error ?? Create(results.Select(r => r.Value));
    }

    public static Result<VehiclePhotosCollection> Create(IEnumerable<VehiclePhoto> photos) =>
        !photos.HasRepeatableValues(p => p.Path, out VehiclePhoto[] repeatable)
            ? new VehiclePhotosCollection(photos)
            : new VehiclePhotosCollectionRepeatableError(photos);
}
