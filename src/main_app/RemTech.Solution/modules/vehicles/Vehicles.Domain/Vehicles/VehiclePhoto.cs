using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePhoto
{
    public string Path { get; }
    
    private VehiclePhoto(string path)
    {
        Path = path;
    }

    public static Result<VehiclePhoto> Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Error.Validation("Путь к фото техники не может быть пустым.");
        return new VehiclePhoto(path);
    }
}