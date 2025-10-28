using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed record VehiclePhoto
{
    public string Path { get; }

    public VehiclePhoto(string path) => Path = path;

    public static Status<VehiclePhoto> Create(string path)
    {
        if (string.IsNullOrEmpty(path))
            return Error.Validation("Путь к фотографии техники был пустым.");
        return new VehiclePhoto(path);
    }
}
