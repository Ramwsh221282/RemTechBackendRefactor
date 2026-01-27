using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Vehicles.Domain.Contracts;

namespace Vehicles.Domain.Categories;

/// <summary>
/// Категория транспортного средства.
/// </summary>
/// <param name="id">Идентификатор категории транспортного средства.</param>
/// <param name="name">Название категории транспортного средства.</param>
public sealed class Category(CategoryId id, CategoryName name) : IPersistable<Category>
{
	/// <summary>
	/// Идентификатор категории транспортного средства.
	/// </summary>
	public CategoryId Id { get; } = id;

	/// <summary>
	/// Название категории транспортного средства.
	/// </summary>
	public CategoryName Name { get; } = name;

	/// <summary>
	/// Сохраняет категорию транспортного средства с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения категории.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения категории.</returns>
	public Task<Result<Category>> SaveBy(IPersister persister, CancellationToken ct = default) =>
		persister.Save(this, ct);
}
