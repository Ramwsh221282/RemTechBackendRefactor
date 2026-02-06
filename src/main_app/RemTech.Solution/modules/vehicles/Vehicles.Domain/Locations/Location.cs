using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Locations;

/// <summary>
/// Локация транспортного средства.
/// </summary>
/// <param name="id">Идентификатор локации.</param>
/// <param name="name">Название локации.</param>
public sealed class Location(LocationId id, LocationName name) : IPersistable<Location>
{
	/// <summary>
	/// Идентификатор локации.
	/// </summary>
	public LocationId Id { get; } = id;

	/// <summary>
	/// Название локации.
	/// </summary>
	public LocationName Name { get; } = name;

	/// <summary>
	/// Сохраняет локацию с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения данных.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения локации.</returns>
	public Task<Result<Location>> SaveBy(IPersister persister, CancellationToken ct = default)
	{
		return persister.Save(this, ct);
	}
}
