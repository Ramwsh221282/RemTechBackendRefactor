using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehiclePhoto
{
    public string Path { get; }

    private VehiclePhoto(string path) => Path = path;

    public static Result<VehiclePhoto> Create(string path) =>
        string.IsNullOrWhiteSpace(path) ? new VehiclePhotoPathEmptyError() : new VehiclePhoto(path);
}
