using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Models;

/// <summary>
/// Модель транспортного средства.
/// </summary>
/// <param name="id">Идентификатор модели транспортного средства.</param>
/// <param name="name">Название модели транспортного средства.</param>
public sealed class Model(ModelId id, ModelName name) : IPersistable<Model>
{
	/// <summary>
	/// Идентификатор модели транспортного средства.
	/// </summary>
	public ModelId Id { get; } = id;

	/// <summary>
	/// Название модели транспортного средства.
	/// </summary>
	public ModelName Name { get; } = name;

	/// <summary>
	/// Сохраняет модель транспортного средства с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения данных.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения модели транспортного средства.</returns>
	public Task<Result<Model>> SaveBy(IPersister persister, CancellationToken ct = default) => persister.Save(this, ct);
}
