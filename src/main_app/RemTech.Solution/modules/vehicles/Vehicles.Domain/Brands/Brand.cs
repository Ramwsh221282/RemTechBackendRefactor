using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Brands;

/// <summary>
/// Марка транспортного средства.
/// </summary>
/// <param name="id">Идентификатор марки.</param>
/// <param name="name">Название марки.</param>
public class Brand(BrandId id, BrandName name) : IPersistable<Brand>
{
	/// <summary>
	/// Идентификатор марки транспортного средства.
	/// </summary>
	public BrandId Id { get; } = id;

	/// <summary>
	/// Название марки транспортного средства.
	/// </summary>
	public BrandName Name { get; } = name;

	/// <summary>
	/// Сохраняет марку транспортного средства с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения марки.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения марки.</returns>
	public Task<Result<Brand>> SaveBy(IPersister persister, CancellationToken ct = default)
	{
		return persister.Save(this, ct);
	}
}
