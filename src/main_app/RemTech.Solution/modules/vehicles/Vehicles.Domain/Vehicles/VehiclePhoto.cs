using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePhoto
{
	private VehiclePhoto(string path)
	{
		Path = path;
	}

	public string Path { get; }

	public static Result<VehiclePhoto> Create(string path)
	{
		return string.IsNullOrWhiteSpace(path)
			? (Result<VehiclePhoto>)Error.Validation("Путь к фото техники не может быть пустым.")
			: (Result<VehiclePhoto>)new VehiclePhoto(path);
	}
}
