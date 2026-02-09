using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Фото транспортного средства.
/// </summary>
public sealed record VehiclePhoto
{
	private VehiclePhoto(string path)
	{
		Path = path;
	}

	/// <summary>
	/// Путь к фото техники.
	/// </summary>
	public string Path { get; }

	/// <summary>
	/// Создаёт фото транспортного средства.
	/// </summary>
	/// <param name="path">Путь к фото транспортного средства.</param>
	/// <returns>Результат создания фото транспортного средства.</returns>
	///
	public static Result<VehiclePhoto> Create(string path)
	{
		return string.IsNullOrWhiteSpace(path)
			? (Result<VehiclePhoto>)Error.Validation("Путь к фото техники не может быть пустым.")
			: (Result<VehiclePhoto>)new VehiclePhoto(path);
	}
}
