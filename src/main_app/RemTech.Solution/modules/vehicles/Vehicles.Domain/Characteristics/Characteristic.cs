using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Characteristics;

/// <summary>
/// Характеристика транспортного средства.
/// </summary>
/// <param name="id">Идентификатор характеристики транспортного средства.</param>
/// <param name="name">Название характеристики транспортного средства.</param>
public sealed class Characteristic(CharacteristicId id, CharacteristicName name) : IPersistable<Characteristic>
{
	/// <summary>
	/// Идентификатор характеристики транспортного средства.
	/// </summary>
	public CharacteristicId Id { get; } = id;

	/// <summary>
	/// Название характеристики транспортного средства.
	/// </summary>
	public CharacteristicName Name { get; } = name;

	/// <summary>
	/// Сохраняет характеристику транспортного средства с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения характеристики.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения характеристики.</returns>
	public Task<Result<Characteristic>> SaveBy(IPersister persister, CancellationToken ct = default) =>
		persister.Save(this, ct);
}
