using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Categories.Contracts;

/// <summary>
/// Персистер категорий.
/// </summary>
public interface ICategoryPersister
{
	/// <summary>
	/// Сохраняет категорию.
	/// </summary>
	/// <param name="category">Категория для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения категории.</returns>
	Task<Result<Category>> Save(Category category, CancellationToken ct = default);
}
