using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Locations;

/// <summary>
/// Идентификатор локации транспортного средства.
/// </summary>
public readonly record struct LocationId
{
	/// <summary>
	/// Создаёт новый идентификатор локации транспортного средства.
	/// </summary>
	public LocationId()
	{
		Id = Guid.NewGuid();
	}

	private LocationId(Guid id)
	{
		Id = id;
	}

	/// <summary>
	/// Уникальный идентификатор локации транспортного средства.
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	/// Создаёт идентификатор локации.
	/// </summary>
	/// <param name="id">Уникальный идентификатор локации.</param>
	/// <returns>Результат создания идентификатора локации.</returns>
	public static Result<LocationId> Create(Guid id)
	{
		return id == Guid.Empty
			? (Result<LocationId>)Error.Validation("Идентификатор локации не может быть пустым.")
			: (Result<LocationId>)new LocationId(id);
	}
}
